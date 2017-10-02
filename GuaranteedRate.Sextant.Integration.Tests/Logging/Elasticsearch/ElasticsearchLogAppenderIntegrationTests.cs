﻿using System.Collections.Generic;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Logging.Elasticsearch;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Integration.Tests.Logging.Elasticsearch
{
    [TestFixture]
    public class ElasticsearchLogAppenderIntegrationTests
    {
        [Test, Category("Integration")]
        public void WhenLog_ThenSuccess()
        {
            using (var sut = new ElasticsearchLogAppender(new IntegrationEncompassConfig()))
            {
                var fields = new Dictionary<string, string>
                {
                    {"Application", "SextantLogger"},
                    {"Company", "Guaranteed Rate"},
                    {"loggerName", "Guaranteed Rate Encompass Logger"}
                };
                sut.Log(fields);
            }
        }
    }
}
