using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Va.Udo.Crm.Queue.Plugins
{
    class EntityLock
    {
        public EntityReference Target { get; set; }
        public string LockField { get; set; }
        public IOrganizationService OrganizationService { get; set; }
        private List<string> _paramters;
        private List<string> Parameters
        {
            get {
                return _paramters;
            }
            set {
                _paramters = new List<string>(value);
                _paramters.Add(LockField);
            }
        }
        public Entity Record { get; private set; }

        public EntityLock()
        {
            _paramters = new List<string>();
        }

        public EntityLock(IOrganizationService service, EntityReference target, string lockfield, IEnumerable<string> parameters=null)
        {
            OrganizationService = service;
            Target = target;
            LockField = lockfield;
            if (parameters != null)
            {
                _paramters = new List<string>(parameters);
            } else {
                _paramters = new List<string>();
            }
            _paramters.Add(LockField);
        }

        public string GetLockCode()
        {
            Record = OrganizationService.Retrieve(Target.LogicalName, Target.Id, new ColumnSet(_paramters.ToArray()));
            return Record.GetAttributeValue<string>(LockField);
        }

        public bool IsLocked()
        {
            return String.IsNullOrWhiteSpace(GetLockCode());
        }

        public bool IsLockedUsing(string lockcode)
        {
            return GetLockCode().Equals(lockcode);
        }

        public static string WaitAndLock(IOrganizationService service, EntityReference target, string lockfield, IEnumerable<string> additionalParams)
        {
            var entityLock = new EntityLock(service, target, lockfield, additionalParams);
            return entityLock.WaitAndLock();
        }

        public void Unlock()
        {
            var e = new Entity(Target.LogicalName);
            e.Id = Target.Id;
            e[LockField] = string.Empty;
            OrganizationService.Update(e);
        }

        public string WaitAndLock(int depth=0)
        {
            if (depth>100) throw new TimeoutException("Unable to lock record after 100 attempts.");
            var lockcode = GetLockCode();
            if (String.IsNullOrWhiteSpace(lockcode))
            {
                // Lock
                var newlockcode = LockRecord();
                // Verify Lock Success
                if (IsLockedUsing(newlockcode)) return newlockcode;
                // Continue if failed
                return WaitAndLock(depth + 1);
            }
            return WaitAndLock(depth + 1);
        }

        private string LockRecord()
        {
            var lockcode = Guid.NewGuid().ToString();
            var entity = new Entity(Target.LogicalName);
            entity.Id = Target.Id;
            entity[LockField] = lockcode;
            OrganizationService.Update(entity);
            return lockcode;
        }

        

        
    }
}
