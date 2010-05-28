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
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Commands.Parameters;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Attributes;

namespace SolrNet.DSL.Tests {
    public class SolrDocument
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("inStock")]
        public bool InStock { get; set; }
        [SolrField("price")]
        public float Price { get; set; }
    }
    [TestFixture]
    public class QueryBuildingTests {

        public string Serialize(object q) {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }
        public QueryBuildingTests()
        {
            Startup.InitContainer();
        }

        [Test]
        public void Simple() {
            var q = Query.Simple("name:solr");
            Assert.AreEqual("name:solr", q.Query);
        }

        [Test]
        public void FieldValue() {
            var q = Query.Field("name").Is("solr");
            Assert.AreEqual("name:solr", Serialize(q));
        }

        [Test]
        public void StronglyTypedFieldValue()
        {
            var q = Query.Field<SolrDocument>(sd => sd.Id).Is("SP2514N");
            Assert.AreEqual("id:SP2514N", Serialize(q));
        }
        [Test]
        public void StronglyTypedFieldValueFloat()
        {   
            var q = Query.Field<SolrDocument>(sd => sd.Id).Is("SP2514N");
            Assert.AreEqual("id:SP2514N", Serialize(q));
        }
        [Test]
        public void StronglyTypedRange()
        {
            var q = Query.Field<SolrDocument>(sd => sd.Price).From(10).To(20);
            Assert.AreEqual("price:[10 TO 20]", Serialize(q));
        }
        [Test]
        public void StronglyTypedFieldValueDecimalWithTypedFieldDefinition()
        {

            var q = Query.Field<SolrDocument, bool>(sd => sd.InStock).Is(true);

            Assert.AreEqual("inStock:True", Serialize(q));
        }
        [Test]
        [Ignore]
        public void StronglyTypedFieldValueDecimalWithTypedField()
        {
            //try an actual test against the example Solr schema
            Startup.Init<SolrDocument>("http://localhost:8983/solr");

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrDocument>>();

            var results = solr.Query(Query.Field<SolrDocument, bool>(sd => sd.InStock).Is(true)
                && Query.Field<SolrDocument, float>(sd => sd.Price).From(0).To(100000), new QueryOptions { Rows = 10 });

            Assert.GreaterThan(results.NumFound, 0);
        }

        [Test]
        public void FieldValueDecimal() {
            var q = Query.Field("price").Is(400);
            Assert.AreEqual("price:400", Serialize(q));
        }

        [Test]
        public void FieldValueNot() {
            var q = Query.Field("name").Is("solr").Not();
            Assert.AreEqual("-name:solr", Serialize(q));
        }

        [Test]
        public void Range() {
            var q = Query.Field("price").From(10).To(20);
            Assert.AreEqual("price:[10 TO 20]", Serialize(q));
        }


        [Test]
        public void InList() {
            var q = Query.Field("price").In(10, 20, 30);
            Assert.AreEqual("(price:10 OR price:20 OR price:30)", Serialize(q));
        }

        [Test]
        public void InList_empty_is_ignored() {
            var q = Query.Field("price").In(new string[0]) && Query.Field("id").Is(123);
            var query = Serialize(q);
            Console.WriteLine(query);
            Assert.AreEqual("(id:123)", query);
        }

        [Test]
        public void HasValue() {
            var q = Query.Field("name").HasAnyValue();
            Assert.AreEqual("name:[* TO *]", Serialize(q));
        }
    }
}