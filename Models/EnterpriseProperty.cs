using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.EnterpriseProperties
{


    public class Settings
    {
        public string termSetId { get; set; }
        public bool allowMultipleValues { get; set; }

    }

    public class ManagedPropertySettings
    {
        public string managedProperty { get; set; }
        public string retrievableManagedProperty { get; set; }
        public string refinableManagedProeprty { get; set; }
        public string queryableManagedProperty { get; set; }
        public string sortableManagedProperty { get; set; }
        public bool retrievable { get; set; }
        public bool refinable { get; set; }
        public bool queryable { get; set; }
        public bool sortable { get; set; }

    }

    public class UiOptions
    {
        public string settingsElementName { get; set; }
        public string editModeElementName { get; set; }
        public string displayModeElementName { get; set; }
        public string valueDefinitionElementName { get; set; }

    }

    public class ValuePropertyPath
    {
        public string path { get; set; }
        public bool isComplex { get; set; }

    }

    public class EnterprisePropertyDataType
    {
        public string id { get; set; }
        public string omniaServiceId { get; set; }
        public string title { get; set; }
        public UiOptions uiOptions { get; set; }
        public ValuePropertyPath valuePropertyPath { get; set; }
        public int indexedType { get; set; }
        public bool omniaSearchable { get; set; }
        public bool spSearchable { get; set; }

    }

    public class EnterpriseProperty
    {
        public string id { get; set; }
        public string internalName { get; set; }
        public Dictionary<string,string> title { get; set; }
        public string enterprisePropertyDataTypeId { get; set; }
        public Settings settings { get; set; }
        public bool spSearchable { get; set; }
        public bool omniaSearchable { get; set; }
        public bool builtIn { get; set; }
        public ManagedPropertySettings managedPropertySettings { get; set; }
        public EnterprisePropertyDataType enterprisePropertyDataType { get; set; }
        public object deletedAt { get; set; }
    }
}