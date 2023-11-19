using CsvHelper;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Infrastructure.Repositories;
using WillBoard.Infrastructure.Repositories.MySql;
using WillBoard.Infrastructure.Services.Instance;
using WillBoard.Infrastructure.TypeHandlers;
using WillBoard.Tools.ImportCountryIp.Models;

namespace WillBoard.Tools.ImportCountryIp
{
    public class Program
    {
        public static async Task Main()
        {
            SqlMapper.AddTypeHandler(new UInt128TypeHandler());

            IHostEnvironment hostEnvironment = new HostingEnvironment
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                EnvironmentName = Environments.Development
            };

            var serviceCollection = new ServiceCollection();

            var configurationService = new InstanceConfigurationService(hostEnvironment);
            serviceCollection.AddSingleton<IConfigurationService>(configurationService);

            serviceCollection.AddSingleton<ISqlConnectionService, MySqlConnectionService>();
            serviceCollection.AddSingleton<ICountryIpRepository, CountryIpRepository>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var sqlConnectionService = serviceProvider.GetService<ISqlConnectionService>();
            var countryIpRepository = serviceProvider.GetService<ICountryIpRepository>();

            if (sqlConnectionService == null)
            {
                Console.WriteLine("ISqlConnectionService is null.");
                throw new NullReferenceException("ISqlConnectionService is null.");
            }

            if (countryIpRepository == null)
            {
                Console.WriteLine("ICountryIpRepository is null.");
                throw new NullReferenceException("ICountryIpRepository is null.");
            }

            var countryIpCollection = new List<CountryIp>();

            Console.WriteLine("Started processing country-ipv4.csv file.");

            using (var reader = new StreamReader($@"{hostEnvironment.ContentRootPath}\Assets\country-ipv4.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var csvCountryIpCollection = csv.GetRecords<CsvCountryIp>();

                foreach (var csvCountryIp in csvCountryIpCollection)
                {
                    var countryIp = new CountryIp();
                    countryIp.IpVersion = IpVersion.IpVersion4;
                    countryIp.IpNumberFrom = UInt128.Parse(csvCountryIp.From);
                    countryIp.IpNumberTo = UInt128.Parse(csvCountryIp.To);
                    countryIp.CountryCode = csvCountryIp.Code;

                    countryIpCollection.Add(countryIp);
                }
            }

            Console.WriteLine("Finished processing country-ipv4.csv file.");
            Console.WriteLine("Started processing country-ipv6.csv file.");

            using (var reader = new StreamReader($@"{hostEnvironment.ContentRootPath}\Assets\country-ipv6.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var csvCountryIpCollection = csv.GetRecords<CsvCountryIp>();

                foreach (var csvCountryIp in csvCountryIpCollection)
                {
                    var countryIp = new CountryIp();
                    countryIp.IpVersion = IpVersion.IpVersion6;
                    countryIp.IpNumberFrom = UInt128.Parse(csvCountryIp.From);
                    countryIp.IpNumberTo = UInt128.Parse(csvCountryIp.To);
                    countryIp.CountryCode = csvCountryIp.Code;

                    countryIpCollection.Add(countryIp);
                }
            }

            Console.WriteLine("Finished processing country-ipv6.csv file.");
            Console.WriteLine("Started processing data in datababase.");

            using (IDbConnection dbConnection = sqlConnectionService.Connection)
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        await countryIpRepository.TruncateAsync(dbConnection, transaction);

                        await countryIpRepository.CreateCollectionAsync(countryIpCollection, dbConnection, transaction);

                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Exception occurred during transaction.");
                        throw new TransactionException("Exception occurred during transaction.", exception);
                    }
                }
            }

            Console.WriteLine("Finished processing data in datababase.");
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}