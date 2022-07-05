using Microsoft.Xrm.Sdk;
using System;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UDO.LOB.Extensions;

namespace UDO.LOB.PeoplelistPayeeCode.CRM
{
    internal class PeopleCollection : IList<UDOPerson>
    {
        private List<UDOPerson> _internal;

        public IEnumerable<string> GetPayeeCodes()
        {
            return _internal.Where(p => !String.IsNullOrEmpty(p.PayeeCode)).Select(p => p.PayeeCode);
        }

        public bool HasPayeeCode(string payeecode)
        {
            if (_internal == null || _internal.Count < 1) return false;
            return _internal.Any(p => !string.IsNullOrEmpty(p.PayeeCode) && p.PayeeCode.Equals(payeecode));
        }

        public bool HasPayeeCode(string payeecode, string pid, SecureString ssn)
        {
            if (_internal == null || _internal.Count < 1) return false;
            return _internal.Any(p => !string.IsNullOrEmpty(p.PayeeCode) && p.PayeeCode.Equals(payeecode)
                && 
                (p.ParticipantId==pid || (p.GetAttributeValue<string>("udo_ssn").ToSecureString().Equals(ssn))));
        }

        internal PeopleCollection()
        {
            _internal = new List<UDOPerson>();
        }

        public PeopleCollection(int capacity)
        {
            _internal = new List<UDOPerson>(capacity);
        }

        public PeopleCollection(IEnumerable<UDOPerson> collection)
        {
            _internal = new List<UDOPerson>(collection);
        }

        public void Clear()
        {
            _internal.Clear();
        }

        public bool Contains(UDOPerson item)
        {
            return _internal.Contains(item);
        }

        public void CopyTo(UDOPerson[] array, int arrayIndex)
        {
            _internal.CopyTo(array, arrayIndex);
        }

        public bool Remove(UDOPerson item)
        {
            return _internal.Remove(item);
        }

        public int Count
        {
            get
            {
                return _internal.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public UDOPerson findPerson(UDOPerson person)
        {
            UDOPerson result = null;

            if (person==null || _internal == null || _internal.Count==0) return null;


            // using a number 1 as true
            bool hasPID = !String.IsNullOrEmpty(person.ParticipantId);

            UDOPerson[] matches = new UDOPerson[2];
            foreach (var item in _internal)
            {
                if (hasPID && !String.IsNullOrEmpty(item.ParticipantId) 
                    && item.ParticipantId.Equals(person.ParticipantId, StringComparison.OrdinalIgnoreCase))
                    return item;
                // both cannot have a pid and match on other criteria
                else if (!(!String.IsNullOrEmpty(item.ParticipantId) && hasPID)
                         && !String.IsNullOrEmpty(person.Name)
                         && !String.IsNullOrEmpty(item.Name)
                         && item.Name.Equals(person.Name, StringComparison.OrdinalIgnoreCase))
                {
                    matches[1] = item;
                    if (!String.IsNullOrEmpty(person.PayeeCode)
                        && !String.IsNullOrEmpty(item.PayeeCode)
                        && item.PayeeCode.Equals(person.PayeeCode, StringComparison.OrdinalIgnoreCase))
                    {
                        matches[0] = item;
                    }
                }
            }
            if (matches[0] != null) return matches[0];
            return matches[1];
        }

        public IEnumerator<UDOPerson> GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        public void Add(UDOPerson person)
        {
            UDOPerson existing = null;
            if ((existing = findPerson(person)) != null)
            {
                person.Clone(existing, true);
                return;
            }
            _internal.Add(person);
        }

        public int IndexOf(UDOPerson item)
        {
            return _internal.IndexOf(item);
        }

        public void Insert(int index, UDOPerson item)
        {
            _internal.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _internal.RemoveAt(index);
        }

        public UDOPerson this[int index]
        {
            get
            {
                return _internal[index];
            }
            set
            {
                _internal[index] = value;
            }
        }

    }
}
