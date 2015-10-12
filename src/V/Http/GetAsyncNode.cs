﻿using System;
using System.ComponentModel.Composition;
using VVVV.PluginInterfaces.V2;
using System.Net.Http;
using VVVV.Core.Logging;

namespace VVVV.Nodes.V.Http
{
    [PluginInfo(Name = "GetAsync")]
    public class GetAsyncNode : IPluginEvaluate, IPartImportsSatisfiedNotification
    {
        [Input("URL", DefaultString = "http://localhost")] 
        protected ISpread<string> FUrlIn;

        [Input("Refresh", IsBang = true)] 
        protected ISpread<bool> FRefreshIn;

        [Output("Body")] 
        protected ISpread<string> FBodyOut;

        [Import()]
        protected ILogger FLogger;

        private HttpClient FClient;

        public void Evaluate(int spreadMax)
        {
            FBodyOut.SliceCount = spreadMax;

            if(FClient == null) return;

            for (var i = 0; i < spreadMax; i++)
            {
                if(!FRefreshIn[i]) continue;

                GetContent(i);
            }
        }

        protected async void GetContent(int index)
        {
            try
            {
                var result = await FClient.GetAsync(new Uri(FUrlIn[index]));
                var content = await result.Content.ReadAsStringAsync();

                FBodyOut[index] = content;
            }
            catch
            {
                FLogger.Log(LogType.Error, "Can't fetch data");
            }
        }

        public void OnImportsSatisfied()
        {
            FClient = new HttpClient();
        }
    }
}