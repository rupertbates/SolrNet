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
using SolrNet.DSL.Impl;
using System.Linq;

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
            MemberExpression operand;
            if (field.Body is MemberExpression)
                operand = (MemberExpression)field.Body;
            else if (field.Body is UnaryExpression)
                operand = (MemberExpression) ((UnaryExpression) field.Body).Operand;
            else 
                throw new ArgumentException("parameter field must be of type MemberExpression or UnaryExpression");

            return new FieldDefinition(operand.Member.Name);
        }
    }
}