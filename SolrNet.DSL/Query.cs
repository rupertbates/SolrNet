#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Linq.Expressions;
using Microsoft.Practices.ServiceLocation;
using SolrNet.DSL.Impl;
using System.Linq;
using SolrNet.Mapping;

namespace SolrNet.DSL {
    public static class Query {
        public static SolrQuery Simple(string s) {
            return new SolrQuery(s);
        }

        public static FieldDefinition Field(string field) {
            return new FieldDefinition(field);
        }
        public static FieldDefinition Field<T>(Expression<Func<T, object>> field)
        {
            return new FieldDefinition(GetSolrField<T>(field.Body).FieldName);
        }
        public static TypedFieldDefinition<TR> Field<T, TR>(Expression<Func<T, TR>> field)
        {
            var solrField = GetSolrField<T>(field);
            if(solrField.Property.PropertyType != typeof(TR))
                throw new ArgumentException(string.Format("The type of the field in the Solr schema ({0}), does not match the type you provided to this method ({1})", solrField.Property.PropertyType, typeof(TR) ));
            return new TypedFieldDefinition<TR>(solrField.FieldName);
        }
        private static MemberExpression GetOperand(Expression expression)
        {
            if (expression is MemberExpression)
                return (MemberExpression) expression;
            
            if (expression is UnaryExpression)
                return (MemberExpression)((UnaryExpression)expression).Operand;
            
            if (expression is LambdaExpression)
                return (MemberExpression) ((LambdaExpression)expression).Body;
            
            throw new ArgumentException("parameter field must be of type MemberExpression or UnaryExpression");
        }
        private static SolrFieldModel GetSolrField<T>(Expression expression)
        {
            var operand = GetOperand(expression);
            var manager = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();

            var fields = manager.GetFields(typeof(T));
            
            var solrField = fields.Where(f => f.Property.Name == operand.Member.Name).FirstOrDefault();
            if (solrField == null)
                throw new ArgumentException(string.Format("The field {0} does not exist in the Solr schema", operand.Member.Name));
            
            return solrField;
        }
    }
}