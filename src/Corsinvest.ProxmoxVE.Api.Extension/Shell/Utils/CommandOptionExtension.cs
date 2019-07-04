using System;
using Corsinvest.ProxmoxVE.Api.Extension.VM;
using McMaster.Extensions.CommandLineUtils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell.Utils
{
    /// <summary>
    /// Command option shell extension.
    /// </summary>
    public static class CommandOptionExtension
    {
        /// <summary>
        /// Add fullname and logo
        /// </summary>
        /// <param name="command"></param>
        public static void AddFullNameLogo(this CommandLineApplication command)
        {
            var parent = command;
            while (parent.Parent != null) { parent = parent.Parent; }

            command.FullName = ShellHelper.MakeLogoAndTitle(parent.Description);
            command.ExtendedHelpText = Environment.NewLine + ShellHelper.REPORT_BUGS;
        }

        /// <summary>
        /// Get option
        /// </summary>
        /// <param name="command"></param>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public static CommandOption GetOption(this CommandLineApplication command, string optionName)
        {
            foreach (var option in command.GetOptions())
            {
                if (option.ShortName == optionName || option.LongName == optionName) { return option; }
            }

            return null;
            //return command.Options.Where(a => a.ShortName == optionName || a.LongName == optionName).FirstOrDefault();
        }

        /// <summary>
        /// Debug is active
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool DebugIsActive(this CommandLineApplication command) => command.GetOption("debug").HasValue();

        /// <summary>
        /// Dryrun is active
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool DryRunIsActive(this CommandLineApplication command) => command.GetOption("dry-run").HasValue();

        /// <summary>
        /// Node option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption NodeOption(this CommandLineApplication command)
            => command.Option("--node", "Node of cluster", CommandOptionType.SingleValue);

        /// <summary>
        /// Debug option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption DebugOption(this CommandLineApplication command)
        {
            var opt = command.Option("--debug", "Debug application", CommandOptionType.NoValue);
            opt.ShowInHelpText = false;
            opt.Inherited = true;
            return opt;
        }

        /// <summary>
        /// Dry run option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption DryRunOption(this CommandLineApplication command)
        {
            var opt = command.Option("--dry-run", "Dry run application", CommandOptionType.NoValue);
            opt.ShowInHelpText = false;
            opt.Inherited = true;
            return opt;
        }

        /// <summary>
        /// Id or name option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption VmIdOrNameOption(this CommandLineApplication command)
            => command.Option("--vmid", "The id or name VM/CT", CommandOptionType.SingleValue).IsRequired();

        /// <summary>
        /// Ids or names option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption VmIdsOrNamesOption(this CommandLineApplication command)
        {
            var opt = command.VmIdOrNameOption();
            opt.Description = @"The id or name VM/CT comma separated (eg. 100,101,102,TestDebian)
-vmid or -name exclude (e.g. -200, -TestUbuntu),
'all-???' for all VM/CT in specific host (e.g. all-pve1, all-\$(hostname)),
'all' for all VM/CT in cluster";

            return opt;
        }

        /// <summary>
        /// VM State option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption VmStateOption(this CommandLineApplication command)
            => command.Option("--state", "Save the vmstate", CommandOptionType.NoValue);

        /// <summary>
        /// Script hook option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption ScriptHookOption(this CommandLineApplication command)
        {
            var opt = command.Option("--script", "Use specified hook script", CommandOptionType.NoValue);
            opt.Accepts().ExistingFile();
            return opt;
        }

        #region Login option
        /// <summary>
        /// Add options login
        /// </summary>
        /// <param name="command"></param>
        public static void AddLoginOptions(this CommandLineApplication command)
        {
            command.HostOption();
            command.UsernameRealOption();
            command.PasswordOption();
        }

        /// <summary>
        /// Host option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption HostOption(this CommandLineApplication command)
            => command.Option("--host", "The host name host[:port]", CommandOptionType.SingleValue).IsRequired();

        /// <summary>
        /// Username real option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption UsernameRealOption(this CommandLineApplication command)
            => command.Option("--username", "User name <username>@<relam>", CommandOptionType.SingleValue).IsRequired();

        /// <summary>
        /// username options
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption Username(this CommandLineApplication command)
            => command.Option("--username", "User name", CommandOptionType.SingleValue).IsRequired();

        /// <summary>
        /// Password option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption PasswordOption(this CommandLineApplication command)
            => command.Option("--password", "The password", CommandOptionType.SingleValue).IsRequired();

        /// <summary>
        /// Get client
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Client Client(this CommandLineApplication command)
        {
            var (host, port) = command.GetHostAndPort();
            var client = new Client(host, port);
            if (command.GetOption("debug").HasValue()) { client.DebugLevel = 99; }
            return client;
        }

        /// <summary>
        /// Get host and port
        /// </summary>
        /// <param name="command"></param>
        /// <param name="defaultPort"></param>
        /// <returns></returns>
        public static (string Host, int Port) GetHostAndPort(this CommandLineApplication command, int defaultPort = 8006)
        {
            var data = command.GetOption("host").Value().Split(':');
            var port = defaultPort;
            if (data.Length == 2) { int.TryParse(data[1], out port); }
            return (data[0], port);
        }

        /// <summary>
        /// Try login client api
        /// </summary>
        /// <param name="command"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool ClientTryLogin(this CommandLineApplication command, Client client)
            => client.Login(command.GetOption("username").Value(), command.GetOption("password").Value());

        /// <summary>
        /// Try login client api
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Client ClientTryLogin(this CommandLineApplication command)
        {
            var client = Client(command);
            if (ClientTryLogin(command, client)) { return client; }
            throw new Exception("Problem connection!");
        }
        #endregion

        /// <summary>
        /// Label option
        /// </summary>
        /// <param name="command"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public static CommandOption LabelOption(this CommandLineApplication command, bool required)
        {
            var opt = command.Option("--label",
                                     "Is usually 'hourly', 'daily', 'weekly', or 'monthly'",
                                     CommandOptionType.SingleValue);
            if (required) { opt.IsRequired(); }
            return opt;
        }

        /// <summary>
        /// Keep option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption<int> KeepOption(this CommandLineApplication command)
            => command.Option<int>("--keep", "Specify the number which should will keep", CommandOptionType.SingleValue)
                      .IsRequired();

        /// <summary>
        /// Mail to option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>                              
        public static CommandOption MailToOption(this CommandLineApplication command)
            => command.Option("--mailto",
                                  "Comma-separated list of email addresses that should receive email notifications",
                                  CommandOptionType.SingleValue)
                      .Accepts(v => v.EmailAddress());

        /// <summary>
        /// Vm Id option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption<int> VmIdOption(this CommandLineApplication command)
            => command.Option<int>("--vmid", "The id VM/CT", CommandOptionType.SingleValue).IsRequired();

        /// <summary>
        /// Wait option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption WaitOption(this CommandLineApplication command)
            => command.Option("--wait", "Wait for task finish", CommandOptionType.NoValue);

        /// <summary>
        /// Option from enum
        /// </summary>
        public static CommandOption OptionEnum<TEnum>(this CommandLineApplication command,
                                                      string template,
                                                      string description) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException("T must be an enumerated type"); }

            var ret = command.Option(template,
                                     description + " " + string.Join(",", Enum.GetNames(typeof(TEnum))),
                                     CommandOptionType.SingleValue);

            ret.Accepts().Enum<TEnum>(true);
            return ret;
        }

        /// <summary>
        /// Get value form enum option
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static TEnum GetEnumValue<TEnum>(this CommandOption command)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException("T must be an enumerated type"); }

            Enum.TryParse<TEnum>(command.Value(), true, out var value);
            return value;
        }

        /// <summary>
        /// Option form string values
        /// </summary>
        /// <param name="command"></param>
        /// <param name="template"></param>
        /// <param name="description"></param>
        /// <param name="allowedValues"></param>
        /// <returns></returns>
        public static CommandOption OptionEnum(this CommandLineApplication command,
                                               string template,
                                               string description,
                                               params string[] allowedValues)
        {
            var ret = command.Option(template,
                                     description + " " + string.Join(",", allowedValues),
                                     CommandOptionType.SingleValue);

            ret.Accepts().Values(true, allowedValues);
            return ret;
        }

        /// <summary>
        /// VM type option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption VMTypeOption(this CommandLineApplication command)
            => command.OptionEnum<VMTypeEnum>("--vmType", "VM type");

        /// <summary>
        /// Snapshot name option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption SnapshotNameOption(this CommandLineApplication command)
            => command.Option("--snapname", "The name of the snapshot", CommandOptionType.SingleValue).IsRequired();

        /// <summary>
        /// Description option
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandOption DescriptionOption(this CommandLineApplication command)
            => command.Option("--description", "A textual description or comment", CommandOptionType.SingleValue)
                      .IsRequired();
    }
}