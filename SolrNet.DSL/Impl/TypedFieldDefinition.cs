using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet.Utils;

namespace SolrNet.DSL.Impl
{
    /// <summary>
    /// A strongly typed FieldDefinition, the reason that this class doesn't inherit from FieldDefinition 
    /// is that we don't want the generic versions of these methods accessible from this class because it 
    /// breaks the type safety.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedFieldDefinition<T>
    {
        private readonly string fieldName;

        public TypedFieldDefinition(string fieldName) {
            this.fieldName = fieldName;
        }

        public RangeDefinition<T> From(T from) {
            return new RangeDefinition<T>(fieldName, from);
        }

        public SolrQueryInList In(params T[] values) {
            return new SolrQueryInList(fieldName, Func.Select(values, v => Convert.ToString(v)));
        }

        public SolrQueryByField Is(T value) {
            return new SolrQueryByField(fieldName, Convert.ToString(value));
        }

        public SolrQueryByRange<string> HasAnyValue() {
            //Need to do this because the generic typing stops 
            //us calling the From and To methods of this class.
            return new FieldDefinition(fieldName).HasAnyValue();
        }
    }
}
