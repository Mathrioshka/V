using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;
using System.Net.Http;

namespace VVVV.Nodes.V
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

        protected HttpClient FClient;

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
            var result = await FClient.GetAsync(new Uri(FUrlIn[index]));
            var content = await result.Content.ReadAsStringAsync();

            FBodyOut[index] = content;
        }

        public void OnImportsSatisfied()
        {
            FClient = new HttpClient();
        }
    }
}
