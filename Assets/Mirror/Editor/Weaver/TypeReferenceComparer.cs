using Mono.CecilX;
using System.Collections.Generic;

namespace Mirror.Weaver {
    /// <summary>
    /// Compares TypeReference using FullName
    /// </summary>
    public class TypeReferenceComparer : IEqualityComparer<TypeReference> {
        public bool Equals(TypeReference x, TypeReference y) {
            return x.FullName == y.FullName;
        }

        public int GetHashCode(TypeReference obj) {
            return obj.FullName.GetHashCode();
        }
    }
}
