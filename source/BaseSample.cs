using System;
using CommandLine;
using Google.Apis.Services;
using System.IO;
using Google.Apis.Http;

namespace ShoppingSamples
{
    /// <summary>
    /// A sample application that runs multiple requests against one of the Shopping APIs.
    /// <list type="bullet">
    /// <item>
    /// <description>Initializes the user credentials</description>
    /// </item>
    /// <item>
    /// <description>Creates the service that queries the API</description>
    /// </item>
    /// <item>
    /// <description>Executes the requests</description>
    /// </item>
    /// </list>
    /// </summary>
    internal abstract class BaseSample
    {
        private static readonly string endpointEnvVar = "GOOGLE_SHOPPING_SAMPLES_ENDPOINT";
        private static readonly string defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "shopping-samples");

        internal abstract string ApiName { get; }
        internal abstract BaseConfig Config { get; }
        internal abstract string Scope { get; }
        internal abstract IClientService Service { get; }
        internal string DefaultPath { get { return defaultPath; } }
        internal Options CliOptions { get; set; }

        internal abstract void initializeConfig(bool noConfig);
        internal abstract void initializeService(BaseClientService.Initializer init);
        internal abstract void initializeService(BaseClientService.Initializer init, Uri u);
        internal abstract void runCalls();

        internal class Options
        {
            // Would like to use the DefaultValue attribute here, but we have to calculate the
            // default path at runtime, which means it can't be used as an attribute value.
            [Option('p', "config_path",
                HelpText = "Configuration directory for Shopping samples.")]
            public string ConfigPath { get; set; }
            [Option('n', "noconfig", DefaultValue = false,
                HelpText = "Run samples without a configuration directory.")]
            public bool NoConfig { get; set; }
        }

        internal void startSamples(string[] args)
        {
            Console.WriteLine("{0} Command Line Sample", ApiName);
            Console.WriteLine("============================================");

            CliOptions = new Options();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, CliOptions);

            if (CliOptions.ConfigPath == null)
            {
                CliOptions.ConfigPath = DefaultPath;
            }

            initializeConfig(CliOptions.NoConfig);

            var initializer = Authenticator.authenticate(Config, Scope);
            if (initializer == null)
            {
                Console.WriteLine("Failed to authenticate, so exiting.");
                return;
            }

            var init = new BaseClientService.Initializer()
            {
                HttpClientInitializer = initializer,
                ApplicationName = ApiName + " Samples",
            };

            if (Environment.GetEnvironmentVariable(endpointEnvVar) == null)
            {
                initializeService(init);
            }
            else
            {
                string url = Environment.GetEnvironmentVariable(endpointEnvVar);
                // BaseUri must have a trailing /.
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
                try
                {
                    var checkedUri = new Uri(url);
                    initializeService(init, checkedUri);
                    Console.WriteLine("Using non-standard API endpoint: {0}", Service.BaseUri);
                }
                catch (UriFormatException e)
                {
                    throw new ArgumentException(
                        String.Format("Error parsing base URL '{0}': {1}", url, e.Message));
                }
            }

            runCalls();
        }
        internal void startTMLewinSamples(string[] args)
        {
            Console.WriteLine("{0} Command Line Sample", this.ApiName);
            Console.WriteLine("============================================");
            this.CliOptions = new Options();
            Parser.Default.ParseArgumentsStrict(args, this.CliOptions, null);
            if (this.CliOptions.ConfigPath == null)
            {
                this.CliOptions.ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "shopping-samples-tmlewin");
            }
            this.initializeConfig(this.CliOptions.NoConfig);
            IConfigurableHttpClientInitializer initializer = Authenticator.authenticate(this.Config, this.Scope);
            if (initializer == null)
            {
                Console.WriteLine("Failed to authenticate, so exiting.");
            }
            else
            {
                BaseClientService.Initializer init = new BaseClientService.Initializer
                {
                    HttpClientInitializer = initializer,
                    ApplicationName = this.ApiName + " Samples"
                };
                if (Environment.GetEnvironmentVariable(endpointEnvVar) == null)
                {
                    this.initializeService(init);
                }
                else
                {
                    string environmentVariable = Environment.GetEnvironmentVariable(endpointEnvVar);
                    if (!environmentVariable.EndsWith("/"))
                    {
                        environmentVariable = environmentVariable + "/";
                    }
                    try
                    {
                        Uri u = new Uri(environmentVariable);
                        this.initializeService(init, u);
                        Console.WriteLine("Using non-standard API endpoint: {0}", this.Service.BaseUri);
                    }
                    catch (UriFormatException exception)
                    {
                        throw new ArgumentException(string.Format("Error parsing base URL '{0}': {1}", environmentVariable, exception.Message));
                    }
                }
                this.runCalls();
            }
        }

    }
}

