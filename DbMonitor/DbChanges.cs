using System;
using System.Configuration;
using Microsoft.AspNet.SignalR.Hubs;
using TableDependency.SqlClient;
using TableDependency.Enums;
using TableDependency;
using WebApplication1.Models;
using TableDependency.EventArgs;
using Microsoft.AspNet.SignalR;
using System.Text;
using Json;
using Newtonsoft.Json;

namespace WebApplication1
{
    public class DbChanges
    {
        private readonly static Lazy<DbChanges> _instance = new Lazy<DbChanges>(
        () => new DbChanges(GlobalHost.ConnectionManager.GetHubContext<DbChangesHub>().Clients));

        private static SqlTableDependency<Change> _tableDependency;

        private static StringBuilder jsonString = new StringBuilder("{'changes':[");

        private DbChanges(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;


            var mapper = new ModelToTableMapper<Change>();
            mapper.AddMapping(c => c.PatientId, "PatientId");

            _tableDependency = new SqlTableDependency<Change>(
                ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString,
                "Db",
                mapper);

            _tableDependency.OnChanged += SqlTableDependency_Changed;
            _tableDependency.OnError += SqlTableDependency_OnError;
            _tableDependency.Start();
        }
        public static DbChanges Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public JsonObject ExportChanges()
        {
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]}");
            string finalString = jsonString.ToString();
            JsonObject final = JsonConvert.DeserializeObject<JsonObject>(finalString);
            jsonString.Clear();
            jsonString.Append("{'changes':[");
            return final;
        }

        void SqlTableDependency_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            throw e.Error;
        }

        void SqlTableDependency_Changed(object sender, RecordChangedEventArgs<Change> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                addString(e.Entity);
            }
        }

        private void addString(Change change)
        {
            jsonString.Append("{'changeType':'" + change.ChangeType + "','name':'" + change.Name + "','patientID':'"
                + change.PatientId + "','numVisits':'" + change.NumVisits + "'},");
        }
    }
}