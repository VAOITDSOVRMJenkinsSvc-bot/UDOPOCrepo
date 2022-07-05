using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace UDO.LOB.PeoplelistPayeeCode.CRM
{
    [DataContractAttribute(Name="Entity", Namespace="http://schemas.microsoft.com/xrm/2011/Contracts"), SuppressMessage("Microsoft.Security", "CA9881:ClassesShouldBeSealed",Justification="This class need to mimic the Entity class that is instantiated by clients.")]
    //[EntityLogicalNameAttribute("udo_person")]
    internal class UDOPerson : Entity, INotifyPropertyChanging, INotifyPropertyChanged, IExtensibleDataObject
    {
        public const string EntityLogicalName = "udo_person";

        
        public UDOPerson()
            : base(EntityLogicalName)
        {
            //PayeeCode = "";
            //this.Name = "";
            //ParticipantId = "";
            //AwardTypeCode = "";
        }


        public UDOPerson Clone(UDOPerson target, bool includeRelatedEntities = false)
        {
            #region remove empty attributes from source to allow for merge
            var toRemove = new List<string>();
            foreach (var attr in Attributes)
            {
                if (attr.Value is string && String.IsNullOrEmpty((string)attr.Value))
                {
                    toRemove.Add(attr.Key);
                }
            }
            foreach (var key in toRemove)
            {
                Attributes.Remove(key);
            }
            #endregion

            var pid = target.ParticipantId;
            var payeeCode = target.PayeeCode;
            var name = target.Name;
            var awardTypeCode = target.AwardTypeCode;

            var sourceEntity = this.ToEntity<Entity>();

            // Implements XrmCloneExtension Entity Cloner
            sourceEntity.Clone(target, includeRelatedEntities);

            // Restore if set to empty (it can happen because Name, PayeeCode, and PID are all
            // properties on UDOPerson, forcing them to be set each time, even if they are empty.

            // If someone has a payeecode other than 00 then the payeecode should be that.
            if (IsVeteran || target.IsVeteran)
            {
                target.PayeeCode = "00";
                target.IsVeteran = true;
                target["udo_type"] = new OptionSetValue(752280000);
            }
            else if (String.IsNullOrEmpty(target.PayeeCode) || target.PayeeCode == "00")
            {
                target.PayeeCode = payeeCode;
            }
            

            if (String.IsNullOrEmpty(target.ParticipantId)) target.ParticipantId = pid;
            if (!target.Name.Equals(name) && !String.IsNullOrEmpty(name))
            {
                var first = target["udo_first"].ToString();
                var last = target["udo_last"].ToString();
                if (name.Length == (first.Length + last.Length + 1))
                {
                    target.Name = name;
                }
                else if (!String.IsNullOrEmpty(first) && !String.IsNullOrEmpty(last))
                {
                    target.Name = (first + " " + last).Trim();
                }
            }
            return target;
        }
        
        public UDOPerson Clean()
        {
            var removeList = new List<string>();
            foreach (var attribute in this.Attributes)
            {
                if (attribute.Key.StartsWith("rem", StringComparison.OrdinalIgnoreCase))
                {
                    removeList.Add(attribute.Key);
                }
            }

            foreach (var key in removeList)
            {
                this.Attributes.Remove(key);
            }
            return this;
        }

        public new object this[string attributeName]
        {
            get
            {
                // This indexer is very simple, and just returns or sets
                // the corresponding element from the internal array.
                return this.Attributes[attributeName];
            }
            set
            {
                // No empty strings allowed to be set on this entity
                this.SetPropertyValue(attributeName, value);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        //public List<string> PayeeCodes { get; set; }

        [IgnoreDataMember]
        public string PayeeCode
        {
            get
            {
                return GetAttributeValue<string>("udo_payeecode");
            }

            set
            {
                this.SetPropertyValue("udo_payeetypecode", value);
                this.SetPropertyValue("udo_payeecode", value);
            }
        }

        [IgnoreDataMember]
        public string AwardTypeCode
        {
            get
            {
                return GetAttributeValue<string>("udo_awardtypecode");
            }

            set
            {
                this.SetPropertyValue("udo_awardtypecode", value);
                this.SetPropertyValue("udo_awardtypecode", value);
            }
        }

        [IgnoreDataMember]
        public bool IsVeteran { get; set; }
        
        [IgnoreDataMember]
        public string Name
        {
            get
            {
                return GetAttributeValue<string>("udo_name");
            }

            set
            {
                this.SetPropertyValue("udo_payeename", value);
                this.SetPropertyValue("udo_name", value);
                
                if (value == null) return;

                if (value.Contains(" "))
                {
                    var names = value.Split(' ');
                    if (names.Length == 2)
                    {
                        this["udo_first"] = names[0];
                        this["udo_last"] = names[1];
                    }
                }

            }
        }

        [IgnoreDataMember]
        public string ParticipantId
        {
            get
            {
                return GetAttributeValue<string>("udo_ptcpntid");
            }

            set
            {
                this.SetPropertyValue("udo_ptcpntid", value);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        private void SetPropertyValue(string name, object value)
        {
            OnPropertyChanging(name);
            Attributes[name] = value;
            //if (name == "udo_payeecode") this.UpdatedPayeeCode(value);
            OnPropertyChanged(name);
        }

        //private void UpdatedPayeeCode(object value)
        //{
        //    var payeeCode = value.ToString();
        //    if (!PayeeCodes.Contains(payeeCode)) PayeeCodes.Add(payeeCode);
        //}
    }
}
