using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MicrosoftSymbolDowner
{
    class RSDSEntity
    {
        public string PDBName { get; }
        public string PDBGuid { get; }
        public string Age { get; }
        public RSDSEntity(string strPDBName, string strPDBGuid, string strAge)
        {
            PDBName = strPDBName;
            PDBGuid = strPDBGuid;
            Age = strAge;
        }
    }

    class PDBEntity
    {
        public RSDSEntity RSDSEntity;
        public string PDBDir { get; }
        public string TmpPDBPath { get; }
        public string PDBPath { get; }
        public string URL { get; }
        public Uri Location { get; set; }
        public PDBEntity(string baseDir, RSDSEntity rsdsEntity)
        {
            RSDSEntity = rsdsEntity;
            PDBDir = Path.Combine(baseDir, rsdsEntity.PDBName, rsdsEntity.PDBGuid + rsdsEntity.Age);
            TmpPDBPath = Path.Combine(PDBDir, rsdsEntity.PDBName + ".tmp");
            PDBPath = Path.Combine(PDBDir, rsdsEntity.PDBName);
            URL = $@"/download/symbols/{rsdsEntity.PDBName}/{rsdsEntity.PDBGuid + rsdsEntity.Age}/{rsdsEntity.PDBName}";
        }
    }
}
