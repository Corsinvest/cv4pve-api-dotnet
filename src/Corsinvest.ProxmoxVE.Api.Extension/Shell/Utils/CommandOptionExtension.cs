using System;
using Corsinvest.ProxmoxVE.Api.Extension.VM;
using McMaster.Extensions.CommandLineUtils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Shell.Utils
{
    public static class CommandOptionExtension
    {
        public static void AddFullNameLogo(this CommandLineApplication command)
        {
            var parent = command;
            while (parent.Parent != null) { parent = parent.Parent; }

            command.FullName = ShellHelper.MakeLogoAndTitle(parent.Description);
            command.ExtendedHelpText = Environment.NewLine + ShellHelper.REPORT_BUGS;
        }

        public static CommandOption GetOption(this CommandLineApplication command, string optionName)
        {
            foreach (var option in command.GetOptions())
            {
                if (option.ShortName == optionName || option.LongName == optionName) { return option; }
            }

            return null;
            //return command.Options.Where(a => a.ShortName == optionName || a.LongName == optionName).FirstOrDefault();
        }

        public static bool DebugActive(this CommandLineApplication command) => command.GetOption("debug").HasValue();
        public static bool DryRunActive(this CommandLineApplication command) => command.GetOption("dry-run").HasValue();

        public static CommandOption NodeOption(this CommandLineApplication command)
        {
            return command.Option("--node", "Node of cluster", CommandOptionType.SingleValue);
        }

        public static CommandOption DebugOption(this CommandLineApplication command)
        {
            var opt = command.Option("--debug", "Debug application", CommandOptionType.NoValue);
            opt.ShowInHelpText = false;
            opt.Inherited = true;
            return opt;
        }

        public static CommandOption DryRunOption(this CommandLineApplication command)
        {
            var opt = command.Option("--dry-run", "Dry run application", CommandOptionType.NoValue);
            opt.ShowInHelpText = false;
            opt.Inherited = true;
            return opt;
        }

        public static CommandOption VmIdOrNameOption(this CommandLineApplication command)
            => command.Option("--vmid", "The id or name VM/CT", CommandOptionType.SingleValue).IsRequired();

        public static CommandOption VmIdsOrNamesOption(this CommandLineApplication command)
        {
            var opt = command.VmIdOrNameOption();
            opt.Description = @"The id or name VM/CT comma separated (eg. 100,101,102,TestDebian)
-vmid or -name exclude (e.g. -200, -TestUbuntu),
'all-???' for all VM/CT in specific host (e.g. all-pve1, all-\$(hostname)),
'all' for all VM/CT in cluster";

            return opt;
        }

        public static CommandOption VmStateOption(this CommandLineApplication command)
            => command.Option("--state", "Save the vmstate", CommandOptionType.NoValue);

        public static CommandOption ScriptHookOption(this CommandLineApplication command)
        {
            var opt = command.Option("--script", "Use specified hook script", CommandOptionType.NoValue);
            opt.Accepts().ExistingFile();
            return opt;
        }

        #region Login option
        public static void AddLoginOptions(this CommandLineApplication command)
        {
            command.HostOption();
            command.UserNameRealOption();
            command.PasswordOption();
        }

        public static CommandOption HostOption(this CommandLineApplication command)
            => command.Option("--host", "The host name host[:port]", CommandOptionType.SingleValue).IsRequired();

        public static CommandOption UserNameRealOption(this CommandLineApplication command)
            => command.Option("--username", "User name <username>@<relam>", CommandOptionType.SingleValue).IsRequired();

        public static CommandOption UserName(this CommandLineApplication command)
            => command.Option("--username", "User name", CommandOptionType.SingleValue).IsRequired();

        public static CommandOption PasswordOption(this CommandLineApplication command)
            => command.Option("--password", "The password", CommandOptionType.SingleValue).IsRequired();

        public static Client Client(this CommandLineApplication command)
        {
            var (host, port) = command.GetHostAndPort(8006);
            var client = new Client(host, port);
            if (command.GetOption("debug").HasValue()) { client.DebugLevel = 99; }
            return client;
        }

        public static (string Host, int Port) GetHostAndPort(this CommandLineApplication command, int defaultPort)
        {
            var data = command.GetOption("host").Value().Split(':');
            var port = defaultPort;
            if (data.Length == 2) { int.TryParse(data[1], out port); }
            return (data[0], port);
        }

        public static bool ClientTryLogin(this CommandLineApplication command, Client client)
            => client.Login(command.GetOption("username").Value(), command.GetOption("password").Value());

        public static Client ClientTryLogin(this CommandLineApplication command)
        {
            var client = Client(command);
            if (ClientTryLogin(command, client)) { return client; }
            throw new Exception("Problem connection!");
        }
        #endregion

        public static CommandOption LabelOption(this CommandLineApplication command, bool required)
        {
            var opt = command.Option("--label",
                                     "Is usually 'hourly', 'daily', 'weekly', or 'monthly'",
                                     CommandOptionType.SingleValue);
            if (required) { opt.IsRequired(); }
            return opt;
        }

        public static CommandOption<int> KeepOption(this CommandLineApplication command)
            => command.Option<int>("--keep", "Specify the number which should will keep", CommandOptionType.SingleValue)
                      .IsRequired();

        public static CommandOption MailToOption(this CommandLineApplication command)
            => command.Option("--mailto",
                                  "Comma-separated list of email addresses that should receive email notifications",
                                  CommandOptionType.SingleValue)
                      .Accepts(v => v.EmailAddress());

        public static CommandOption<int> VmIdOption(this CommandLineApplication command)
            => command.Option<int>("--vmid", "The id VM/CT", CommandOptionType.SingleValue).IsRequired();

        public static CommandOption WaitOption(this CommandLineApplication command)
            => command.Option("--wait", "Wait for task finish", CommandOptionType.NoValue);

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

        public static TEnum GetEnumValue<TEnum>(this CommandOption command)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum) { throw new ArgumentException("T must be an enumerated type"); }

            Enum.TryParse<TEnum>(command.Value(), true, out var value);
            return value;
        }

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

        public static CommandOption VMTypeOption(this CommandLineApplication command)
            => command.OptionEnum<VMTypeEnum>("--vmType", "VM type");

        public static CommandOption SnapshotNameOption(this CommandLineApplication command)
            => command.Option("--snapname", "The name of the snapshot", CommandOptionType.SingleValue).IsRequired();

        public static CommandOption DescriptionOption(this CommandLineApplication command)
            => command.Option("--description", "A textual description or comment", CommandOptionType.SingleValue)
                      .IsRequired();
    }
}