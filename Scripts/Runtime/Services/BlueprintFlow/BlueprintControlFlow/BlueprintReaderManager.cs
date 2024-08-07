namespace GameCore.Services.BlueprintFlow.BlueprintControlFlow
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
    using GameCore.Services.BlueprintFlow.APIHandler;
    using GameCore.Services.BlueprintFlow.BlueprintReader;
    using GameCore.Services.BlueprintFlow.Message;
    using GameCore.Services.Logger;
    using GameCore.Services.Message;
    using GameCore.Services.UserData.Interface;
    using GameExtensions.Reflection;
    using UnityEngine;
    using VContainer;

    /// <summary>
    ///  The main manager for reading blueprints pipeline/>.
    /// </summary>
    public class BlueprintReaderManager
    {
#region zeject

        private readonly IMessageService     messageService;
        private readonly IObjectResolver     resolver;
        private readonly IUserDataService    userDataServices;
        private readonly BlueprintConfig     blueprintConfig;
        private readonly FetchBlueprintInfo  fetchBlueprintInfo;
        private readonly BlueprintDownloader blueprintDownloader;

#endregion

        private readonly BlueprintProgressMessage blueprintProgressMessage = new();

        public BlueprintReaderManager
        (
            IMessageService     messageService,
            IObjectResolver     resolver,
            IUserDataService    userDataServices,
            BlueprintConfig     blueprintConfig,
            FetchBlueprintInfo  fetchBlueprintInfo,
            BlueprintDownloader blueprintDownloader
        )
        {
            this.messageService      = messageService;
            this.resolver            = resolver;
            this.userDataServices    = userDataServices;
            this.blueprintConfig     = blueprintConfig;
            this.fetchBlueprintInfo  = fetchBlueprintInfo;
            this.blueprintDownloader = blueprintDownloader;
        }

        public virtual async UniTask LoadBlueprint()
        {
            LoggerUtils.Log("[BlueprintReader] Start loading");
            Dictionary<string, string> listRawBlueprints = null;
            if (this.blueprintConfig.IsResourceMode)
            {
                listRawBlueprints = new Dictionary<string, string>();
                this.messageService.Publish(new LoadBlueprintDataProgressMessage { Percent = 1f });
            }
            else
            {
                var newBlueprintInfo = await this.fetchBlueprintInfo.GetBlueprintInfo(this.blueprintConfig.FetchBlueprintUri);
                if (!await this.IsCachedBlueprintUpToDate(newBlueprintInfo.Url, newBlueprintInfo.Hash))
                {
                    await this.DownloadBlueprint(newBlueprintInfo.Url);
                }

                //Is blueprint zip file exists in storage
                if (File.Exists(this.blueprintConfig.BlueprintZipFilepath))
                {
                    // Save blueprint info to local
                    this.userDataServices.Save<BlueprintInfoData>();

                    // Unzip file to memory
#if !UNITY_WEBGL
                    listRawBlueprints = await UniTask.RunOnThreadPool(this.UnzipBlueprint);

#else
                    listRawBlueprints = await UniTask.Create(this.UnzipBlueprint);
#endif
                }
            }

            if (listRawBlueprints == null)
            {
                //Show warning popup
                return;
            }

            //Load all blueprints to instances
            try
            {
                await this.ReadAllBlueprint(listRawBlueprints);
            }
            catch (Exception e)
            {
                LoggerUtils.Exception(e);
            }

            LoggerUtils.Log("[BlueprintReader] All blueprint are loaded");

            this.messageService.Publish(new LoadBlueprintDataSucceedMessage());
        }

        protected virtual async UniTask<bool> IsCachedBlueprintUpToDate(string url, string hash)
        {
            var blueprintInfoData = await this.userDataServices.Load<BlueprintInfoData>();

            return blueprintInfoData.Url.Equals(url) && MD5Utils.GetMD5HashFromFile(this.blueprintConfig.BlueprintZipFilepath).Equals(hash);
        }

        //Download new blueprints version from remote
        private async UniTask DownloadBlueprint(string blueprintDownloadLink)
        {
            var progressSignal = new LoadBlueprintDataProgressMessage { Percent = 0f };
            this.messageService.Publish(progressSignal); //Inform that we just starting dowloading blueprint
            await this.blueprintDownloader.DownloadBlueprintAsync(blueprintDownloadLink, this.blueprintConfig.BlueprintZipFilepath, (downloaded, length) =>
                                                                                                                                    {
                                                                                                                                        progressSignal.Percent = downloaded / (float)length * 100f;
                                                                                                                                        this.messageService.Publish(progressSignal);
                                                                                                                                    });
        }

        protected virtual async UniTask<Dictionary<string, string>> UnzipBlueprint()
        {
            var result = new Dictionary<string, string>();
            if (!File.Exists(this.blueprintConfig.BlueprintZipFilepath))
            {
                return result;
            }

            using var archive = ZipFile.OpenRead(this.blueprintConfig.BlueprintZipFilepath);
            foreach (var entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith(this.blueprintConfig.BlueprintFileType, StringComparison.OrdinalIgnoreCase))
                    continue;
                using var streamReader   = new StreamReader(entry.Open());
                var       readToEndAsync = await streamReader.ReadToEndAsync();
                result.Add(entry.Name, readToEndAsync);
            }

            return result;
        }

        private UniTask ReadAllBlueprint(Dictionary<string, string> listRawBlueprints)
        {
            if (!File.Exists(this.blueprintConfig.BlueprintZipFilepath))
            {
                LoggerUtils.Warning($"[BlueprintReader] {this.blueprintConfig.BlueprintZipFilepath} is not exists!!!, Continue load from resource");
            }

            var listReadTask    = new List<UniTask>();
            var allDerivedTypes = AppDomain.CurrentDomain.GetAllTypeFromDerived<IGenericBlueprintReader>();

            this.blueprintProgressMessage.MaxBlueprint    = allDerivedTypes.Count();
            this.blueprintProgressMessage.CurrentProgress = 0;
            this.messageService.Publish(this.blueprintProgressMessage); // Inform that we just start reading blueprint
            foreach (var blueprintType in allDerivedTypes)
            {
                var blueprintInstance = (IGenericBlueprintReader)this.resolver.Resolve(blueprintType);
                if (blueprintInstance != null)
                {
#if !UNITY_WEBGL
                    listReadTask.Add(UniTask.RunOnThreadPool(() => this.OpenReadBlueprint(blueprintInstance, listRawBlueprints)));
#else
                    listReadTask.Add(UniTask.Create(() => this.OpenReadBlueprint(blueprintInstance, listRawBlueprints)));
#endif
                }
                else
                {
                    LoggerUtils.Log($"Can not resolve blueprint {blueprintType.Name}");
                }
            }

            return UniTask.WhenAll(listReadTask);
        }

        private async UniTask OpenReadBlueprint(IGenericBlueprintReader blueprintReader, Dictionary<string, string> listRawBlueprints)
        {
            var bpAttribute = blueprintReader.GetType().GetCustomAttribute<BlueprintReaderAttribute>();
            if (bpAttribute != null)
            {
                if (bpAttribute.BlueprintScope == BlueprintScope.Server) return;

                // Try to load a raw blueprint file from local or resource folder
                string rawCsv;
                if (this.blueprintConfig.IsResourceMode || bpAttribute.IsLoadFromResource)
                {
                    rawCsv = await LoadRawCsvFromResourceFolder();
                }
                else
                {
                    if (!listRawBlueprints.TryGetValue(bpAttribute.DataPath + this.blueprintConfig.BlueprintFileType, out rawCsv))
                    {
                        LoggerUtils.Warning($"[BlueprintReader] Blueprint {bpAttribute.DataPath} is not exists at the local folder, try to load from resource folder");
                        rawCsv = await LoadRawCsvFromResourceFolder();
                    }
                }

                async UniTask<string> LoadRawCsvFromResourceFolder()
                {
                    await UniTask.SwitchToMainThread();
                    var result = string.Empty;
                    try
                    {
                        result = ((TextAsset)await Resources.LoadAsync<TextAsset>(this.blueprintConfig.ResourceBlueprintPath + bpAttribute.DataPath)).text;
                    }
                    catch (Exception e)
                    {
                        LoggerUtils.Error($"Load {bpAttribute.DataPath} blueprint error!!!");
                        LoggerUtils.Exception(e);
                    }

#if !UNITY_WEBGL
                    await UniTask.SwitchToThreadPool();
#endif
                    return result;
                }

                // Deserialize the raw blueprint to the blueprint reader instance

                if (!string.IsNullOrEmpty(rawCsv))
                {
                    await blueprintReader.DeserializeFromCsv(rawCsv);
                    lock (this.blueprintProgressMessage)
                    {
                        this.blueprintProgressMessage.CurrentProgress++;
                        this.messageService.Publish(this.blueprintProgressMessage);
                    }
                }
                else
                    LoggerUtils.Warning($"[BlueprintReader] Unable to load {bpAttribute.DataPath} from {(bpAttribute.IsLoadFromResource ? "resource folder" : "local folder")}!!!");
            }
            else
            {
                LoggerUtils.Warning($"[BlueprintReader] Class {blueprintReader} does not have BlueprintReaderAttribute yet");
            }
        }
    }
}