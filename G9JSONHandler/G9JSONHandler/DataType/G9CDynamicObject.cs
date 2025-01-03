using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace G9JSONHandler.DataType
{
    public class G9CDynamicObject : DynamicObject, IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public object this[string key]
        {
            get => _properties.ContainsKey(key) ? _properties[key] : null;
            set => _properties[key] = value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _properties.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }
    }
}