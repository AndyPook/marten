﻿using System;

namespace Marten.Schema.Sequences
{
    public class SequenceFactory : ISequences
    {
        private readonly IDocumentSchema _schema;
        private readonly IConnectionFactory _factory;
        private readonly StoreOptions _options;
        private readonly IMartenLogger _logger;

        public SequenceFactory(IDocumentSchema schema, IConnectionFactory factory, StoreOptions options, IMartenLogger logger)
        {
            _schema = schema;
            _factory = factory;
            _options = options;
            _logger = logger;
        }

        public ISequence Hilo(Type documentType, HiloSettings settings)
        {
            

            if (!_schema.TableExists(_options.DatabaseSchemaName, "mt_hilo"))
            {
                if (_options.AutoCreateSchemaObjects == AutoCreate.None)
                {
                    throw new InvalidOperationException($"Hilo table is missing, but {nameof(StoreOptions.AutoCreateSchemaObjects)} is {_options.AutoCreateSchemaObjects}");
                }

                var sqlScript = SchemaBuilder.GetSqlScript(_options.DatabaseSchemaName, "mt_hilo");
                _logger.SchemaChange(sqlScript);

                _factory.RunSql(sqlScript);
            }

            return new HiloSequence(_factory, _options, documentType.Name, settings);
        }
    }
}