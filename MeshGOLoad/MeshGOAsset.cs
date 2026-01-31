using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DingoAssetsLoadSystem.MeshGOLoad
{
    public sealed class MeshGOAsset
    {
        public GameObject PrototypeRoot;
        public readonly List<Object> OwnedObjects = new();
        
        public IDisposable Importer;
    }
}