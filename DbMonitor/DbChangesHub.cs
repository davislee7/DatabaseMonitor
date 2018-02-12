using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using WebApplication1.Models;
using Json;
namespace WebApplication1
{
    public class DbChangesHub : Hub
    {
        private readonly DbChanges _dbChanges;

        public DbChangesHub() : this(DbChanges.Instance)
        { }

        public DbChangesHub(DbChanges dbChanges)
        {
            _dbChanges = dbChanges;
        }

        public JsonObject GetAllChanges()
        {
            return _dbChanges.ExportChanges();
        }
        
    }
}