﻿// reference:System.dll
// reference:System.Core.dll
// reference:System.Web.Extensions.dll

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Advanced_Combat_Tracker;
using DFAssist.DataModel;

namespace DFAssist
{
    public class MainControl : UserControl, IActPluginV1
    {
        private readonly string _settingsFile;
        private readonly ConcurrentStack<string> _telegramSelectedFates;
        private readonly ConcurrentDictionary<int, ProcessNet> _networks;

        private bool _isPluginEnabled;
        private bool _lockTreeEvent;
        private bool _pluginInitializing;
        private bool _isDutyAlertEnabled;
        private bool _isTelegramEnabled;
        private bool _mainFormIsLoaded;
        private bool _isToastNotificationEnabled;
        private string _checkedFates;
        private Timer _timer;
        private Label _label1;
        private Label _label2;
        private Label _label3;
        private Label _label4;
        private Label _labelStatus;
        private Language _selectedLanguage;
        private TextBox _telegramChatIdTextBox;
        private TextBox _telegramTokenTextBox;
        private CheckBox _telegramCheckBox;
        private CheckBox _dutyFinderAlertCheckBox;
        private CheckBox _toadNotificationCheckBox;
        private ComboBox _languageComboBox;
        private GroupBox _groupBox1;
        private GroupBox _groupBox2;
        private SettingsSerializer _xmlSettingsSerializer;
        private GroupBox _groupBox3;
        private RichTextBox _richTextBox1;
        private CheckBox _enableLoggingCheckBox;
        private Button _button1;

        public TreeView TelegramFateTreeView;

        #region WinForm Required
        public MainControl()
        {
            InitializeComponent();
            Logger.SetTextBox(_richTextBox1);
            _settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config", "DFAssist.config.xml");

            _networks = new ConcurrentDictionary<int, ProcessNet>();
            _telegramSelectedFates = new ConcurrentStack<string>();

            ActGlobals.oFormActMain.Load += ActMainFormOnLoad;
        }

        private void MonitorProcess()
        {
            if (_timer == null)
            {
                _timer = new Timer {Interval = 30000};
                _timer.Tick += Timer_Tick;
            }

            _timer.Enabled = true;

            UpdateProcesses();
        }

        /// <summary>
        /// This is autmatically generated from the designer
        /// </summary>
        private void InitializeComponent()
        {
            _label1 = new Label();
            _languageComboBox = new ComboBox();
            _groupBox1 = new GroupBox();
            _telegramTokenTextBox = new TextBox();
            _label3 = new Label();
            _telegramChatIdTextBox = new TextBox();
            _label2 = new Label();
            _telegramCheckBox = new CheckBox();
            _groupBox2 = new GroupBox();
            _label4 = new Label();
            TelegramFateTreeView = new TreeView();
            _dutyFinderAlertCheckBox = new CheckBox();
            _toadNotificationCheckBox = new CheckBox();
            _groupBox3 = new GroupBox();
            _enableLoggingCheckBox = new CheckBox();
            _button1 = new Button();
            _richTextBox1 = new RichTextBox();
            _groupBox1.SuspendLayout();
            _groupBox2.SuspendLayout();
            _groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // _label1
            // 
            _label1.AutoSize = true;
            _label1.Location = new Point(21, 17);
            _label1.Name = "_label1";
            _label1.Size = new Size(0, 13);
            _label1.TabIndex = 7;
            _label1.Text = @"Language";
            // 
            // _languageComboBox
            // 
            _languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _languageComboBox.FormattingEnabled = true;
            _languageComboBox.Location = new Point(88, 14);
            _languageComboBox.Name = "_languageComboBox";
            _languageComboBox.Size = new Size(121, 21);
            _languageComboBox.TabIndex = 6;
            _languageComboBox.SelectedValueChanged += LanguageComboBox_SelectedValueChanged;
            // 
            // _groupBox1
            // 
            _groupBox1.Controls.Add(_telegramTokenTextBox);
            _groupBox1.Controls.Add(_label3);
            _groupBox1.Controls.Add(_telegramChatIdTextBox);
            _groupBox1.Controls.Add(_label2);
            _groupBox1.Controls.Add(_telegramCheckBox);
            _groupBox1.Location = new Point(23, 49);
            _groupBox1.Name = "_groupBox1";
            _groupBox1.Size = new Size(533, 51);
            _groupBox1.TabIndex = 9;
            _groupBox1.TabStop = false;
            _groupBox1.Text = @"Enable Telegram Notifications";
            // 
            // _telegramTokenTextBox
            // 
            _telegramTokenTextBox.Location = new Point(232, 20);
            _telegramTokenTextBox.Name = "_telegramTokenTextBox";
            _telegramTokenTextBox.Size = new Size(291, 20);
            _telegramTokenTextBox.TabIndex = 9;
            // 
            // _label3
            // 
            _label3.AutoSize = true;
            _label3.Location = new Point(186, 23);
            _label3.Name = "_label3";
            _label3.Size = new Size(38, 13);
            _label3.TabIndex = 8;
            _label3.Text = @"Token";
            // 
            // _telegramChatIdTextBox
            // 
            _telegramChatIdTextBox.Location = new Point(67, 20);
            _telegramChatIdTextBox.Name = "_telegramChatIdTextBox";
            _telegramChatIdTextBox.Size = new Size(100, 20);
            _telegramChatIdTextBox.TabIndex = 7;
            // 
            // _label2
            // 
            _label2.AutoSize = true;
            _label2.Location = new Point(15, 23);
            _label2.Name = "_label2";
            _label2.Size = new Size(41, 13);
            _label2.TabIndex = 6;
            _label2.Text = @"Chat Id";
            // 
            // _telegramCheckBox
            // 
            _telegramCheckBox.AutoSize = true;
            _telegramCheckBox.Location = new Point(180, 0);
            _telegramCheckBox.Name = "_telegramCheckBox";
            _telegramCheckBox.Size = new Size(15, 14);
            _telegramCheckBox.TabIndex = 5;
            _telegramCheckBox.UseVisualStyleBackColor = true;
            _telegramCheckBox.CheckedChanged += CheckBoxTelegram_CheckedChanged;
            // 
            // _groupBox2
            // 
            _groupBox2.Controls.Add(_label4);
            _groupBox2.Controls.Add(TelegramFateTreeView);
            _groupBox2.Controls.Add(_dutyFinderAlertCheckBox);
            _groupBox2.Location = new Point(23, 115);
            _groupBox2.Name = "_groupBox2";
            _groupBox2.Size = new Size(533, 457);
            _groupBox2.TabIndex = 10;
            _groupBox2.TabStop = false;
            _groupBox2.Text = @"Alerts";
            // 
            // _label4
            // 
            _label4.AutoSize = true;
            _label4.Location = new Point(15, 57);
            _label4.Name = "_label4";
            _label4.Size = new Size(43, 13);
            _label4.TabIndex = 10;
            _label4.Text = @"F.A.T.E";
            // 
            // TelegramFateTreeView
            // 
            TelegramFateTreeView.CheckBoxes = true;
            TelegramFateTreeView.Location = new Point(15, 81);
            TelegramFateTreeView.Name = "TelegramFateTreeView";
            TelegramFateTreeView.Size = new Size(508, 370);
            TelegramFateTreeView.TabIndex = 9;
            TelegramFateTreeView.AfterCheck += FateTreeView_AfterCheck;
            // 
            // _dutyFinderAlertCheckBox
            // 
            _dutyFinderAlertCheckBox.AutoSize = true;
            _dutyFinderAlertCheckBox.Checked = true;
            _dutyFinderAlertCheckBox.CheckState = CheckState.Checked;
            _dutyFinderAlertCheckBox.Location = new Point(15, 22);
            _dutyFinderAlertCheckBox.Name = "_dutyFinderAlertCheckBox";
            _dutyFinderAlertCheckBox.Size = new Size(80, 17);
            _dutyFinderAlertCheckBox.TabIndex = 8;
            _dutyFinderAlertCheckBox.Text = @"Duty Finder";
            _dutyFinderAlertCheckBox.UseVisualStyleBackColor = true;
            _dutyFinderAlertCheckBox.CheckedChanged += CheckBoxDutyFinder_CheckedChanged;
            // 
            // _toadNotificationCheckBox
            // 
            _toadNotificationCheckBox.AutoSize = true;
            _toadNotificationCheckBox.Location = new Point(255, 18);
            _toadNotificationCheckBox.Name = "_toadNotificationCheckBox";
            _toadNotificationCheckBox.Size = new Size(142, 17);
            _toadNotificationCheckBox.TabIndex = 11;
            _toadNotificationCheckBox.Text = @"Enable Toast Notifications";
            _toadNotificationCheckBox.UseVisualStyleBackColor = true;
            _toadNotificationCheckBox.CheckedChanged += ToastNotificationCheckBox_CheckedChanged;
            // 
            // _groupBox3
            // 
            _groupBox3.Controls.Add(_enableLoggingCheckBox);
            _groupBox3.Controls.Add(_button1);
            _groupBox3.Controls.Add(_richTextBox1);
            _groupBox3.Location = new Point(563, 49);
            _groupBox3.Name = "_groupBox3";
            _groupBox3.Size = new Size(710, 523);
            _groupBox3.TabIndex = 12;
            _groupBox3.TabStop = false;
            _groupBox3.Text = @"Logs";
            // 
            // _enableLoggingCheckBox
            // 
            _enableLoggingCheckBox.AutoSize = true;
            _enableLoggingCheckBox.Checked = true;
            _enableLoggingCheckBox.CheckState = CheckState.Checked;
            _enableLoggingCheckBox.Location = new Point(7, 22);
            _enableLoggingCheckBox.Name = "_enableLoggingCheckBox";
            _enableLoggingCheckBox.Size = new Size(100, 17);
            _enableLoggingCheckBox.TabIndex = 2;
            _enableLoggingCheckBox.Text = @"Enable Logging";
            _enableLoggingCheckBox.UseVisualStyleBackColor = true;
            _enableLoggingCheckBox.CheckedChanged += EnableLoggingCheckBox_CheckedChanged;
            // 
            // _button1
            // 
            _button1.Location = new Point(580, 20);
            _button1.Name = "_button1";
            _button1.AutoSize = true;
            _button1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _button1.TabIndex = 1;
            _button1.Text = @"Clear Logs";
            _button1.UseVisualStyleBackColor = true;
            _button1.Click += ClearLogsButton_Click;
            // 
            // _richTextBox1
            // 
            _richTextBox1.Location = new Point(6, 52);
            _richTextBox1.Name = "_richTextBox1";
            _richTextBox1.ReadOnly = true;
            _richTextBox1.Size = new Size(698, 465);
            _richTextBox1.TabIndex = 0;
            _richTextBox1.Text = "";
            // 
            // MainControl
            // 
            Controls.Add(_groupBox3);
            Controls.Add(_toadNotificationCheckBox);
            Controls.Add(_groupBox2);
            Controls.Add(_groupBox1);
            Controls.Add(_label1);
            Controls.Add(_languageComboBox);
            Name = "MainControl";
            Size = new Size(1744, 592);
            _groupBox1.ResumeLayout(false);
            _groupBox1.PerformLayout();
            _groupBox2.ResumeLayout(false);
            _groupBox2.PerformLayout();
            _groupBox3.ResumeLayout(false);
            _groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion

        #region IActPluginV1 Implementations
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            if (_pluginInitializing)
                return;

            _pluginInitializing = true;
            _isPluginEnabled = true;
            _labelStatus = pluginStatusText;

            _languageComboBox.DataSource = new[]
            {
                    new Language {Name = "English", Code = "en-us"},
                    new Language {Name = "한국어", Code = "ko-kr"},
                    new Language {Name = "日本語", Code = "ja-jp"},
                    new Language {Name = "Français", Code = "fr-fr"}
            };

            _languageComboBox.DisplayMember = "Name";
            _languageComboBox.ValueMember = "Code";

            _labelStatus.Text = @"Starting...";

            LoadData();
            UpdateTranslations();

            _labelStatus.Text = Localization.GetText("l-plugin-started");
            pluginScreenSpace.Text = Localization.GetText("app-name");

            pluginScreenSpace.Controls.Add(this);
            _xmlSettingsSerializer = new SettingsSerializer(this);

            LoadSettings();
            LoadFates();

            if(_mainFormIsLoaded)
                MonitorProcess();

            _pluginInitializing = false;
        }

        public void DeInitPlugin()
        {
            _isPluginEnabled = false;
            Logger.SetTextBox(null);

            if (_labelStatus != null)
            {
                _labelStatus.Text = Localization.GetText("l-plugin-stopped");
                _labelStatus = null;
            }

            foreach (var entry in _networks)
            {
                entry.Value.Network.StopCapture();
            }

            _timer.Enabled = false;
            ActGlobals.oFormActMain.Load -= ActMainFormOnLoad;

            SaveSettings();
        }
        #endregion

        #region Getters
        private static string GetInstanceName(int code)
        {
            return Data.GetInstance(code).Name;
        }

        private static string GetFateName(int code)
        {
            return Data.GetFate(code).Name;
        }

        private static string GetAreaNameFromFate(int code)
        {
            return Data.GetFate(code).Area.Name;
        }

        private static string GetRouletteName(int code)
        {
            return Data.GetRoulette(code).Name;
        }
        #endregion

        #region Load Methods
        private void LoadData()
        {
            var newLanguage = (Language)_languageComboBox.SelectedItem;
            if (_selectedLanguage != null && newLanguage.Code.Equals(_selectedLanguage.Code))
                return;

            _selectedLanguage = newLanguage;
            Localization.Initialize(_selectedLanguage.Code);
            Data.Initialize(_selectedLanguage.Code);
        }

        private void LoadFates()
        {
            TelegramFateTreeView.Nodes.Clear();

            var checkedFates = new List<string>();
            if (!string.IsNullOrEmpty(_checkedFates))
            {
                var split = _checkedFates.Split('|');
                checkedFates.AddRange(split);
            }

            _lockTreeEvent = true;

            foreach (var area in Data.Areas)
            {
                var areaNode = TelegramFateTreeView.Nodes.Add(area.Value.Name);
                areaNode.Tag = "AREA:" + area.Key;

                if (checkedFates.Contains((string)areaNode.Tag))
                    areaNode.Checked = true;

                foreach (var fate in area.Value.Fates)
                {
                    var fateName = fate.Value.Name;
                    var fateNode = areaNode.Nodes.Add(fateName);
                    fateNode.Tag = fate.Key.ToString();

                    if (checkedFates.Contains((string)fateNode.Tag))
                        fateNode.Checked = true;
                }
            }

            _telegramSelectedFates.Clear();
            UpdateSelectedFates(TelegramFateTreeView.Nodes);
            _lockTreeEvent = false;
        }
        #endregion

        #region Update Methods
        private void UpdateProcesses()
        {
            var processes = new List<Process>();
            processes.AddRange(Process.GetProcessesByName("ffxiv"));
            processes.AddRange(Process.GetProcessesByName("ffxiv_dx11"));

            foreach (var process in processes)
            {
                try
                {
                    if (_networks.ContainsKey(process.Id))
                        continue;

                    var pn = new ProcessNet(process, new Network());
                    FFXIVPacketHandler.OnEventReceived += Network_onReceiveEvent;
                    _networks.TryAdd(process.Id, pn);
                    Logger.Success("l-process-set-success", process.Id);
                }
                catch (Exception e)
                {
                    Logger.Exception(e, "l-process-set-failed");
                }
            }

            var toDelete = new List<int>();
            foreach (var entry in _networks)
            {
                if (entry.Value.Process.HasExited)
                {
                    entry.Value.Network.StopCapture();
                    toDelete.Add(entry.Key);
                }
                else
                {
                    if (entry.Value.Network.IsRunning)
                        entry.Value.Network.UpdateGameConnections(entry.Value.Process);
                    else
                        entry.Value.Network.StartCapture(entry.Value.Process);
                }
            }

            foreach (var t in toDelete)
            {
                try
                {
                    _networks.TryRemove(t, out var _);
                    FFXIVPacketHandler.OnEventReceived -= Network_onReceiveEvent;
                }
                catch (Exception e)
                {
                    Logger.Exception(e, "l-process-remove-failed");
                }
            }
        }

        private void UpdateSelectedFates(IEnumerable nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                    _telegramSelectedFates.Push((string)node.Tag);

                UpdateSelectedFates(node.Nodes);
            }
        }

        private void UpdateTranslations()
        {
            _label1.Text = Localization.GetText("ui-language-display-text");
            _groupBox1.Text = Localization.GetText("ui-telegram-display-text");
            _label3.Text = Localization.GetText("ui-telegram-token-display-text");
            _label2.Text = Localization.GetText("ui-telegram-chatid-display-text");
            _groupBox2.Text = Localization.GetText("ui-alerts-display-text");
            _label4.Text = Localization.GetText("ui-alerts-fate-display-text");
            _dutyFinderAlertCheckBox.Text = Localization.GetText("ui-alerts-dutyfinder-display-text");
            _toadNotificationCheckBox.Text = Localization.GetText("ui-toast-notification-display-text");
            _groupBox3.Text = Localization.GetText("ui-log-display-text");
            _enableLoggingCheckBox.Text = Localization.GetText("ui-log-enable-display-text");
            _button1.Text = Localization.GetText("ui-log-clear-display-text");
        }
        #endregion

        #region Post Method
        private static void SendToAct(string text)
        {
            ActGlobals.oFormActMain.ParseRawLogLine(false, DateTime.Now, "00|" + DateTime.Now.ToString("O") + "|0048|F|" + text);
        }

        private void PostToTelegramIfNeeded(string server, EventType eventType, int[] args)
        {
            if (eventType != EventType.FATE_BEGIN && eventType != EventType.MATCH_ALERT) return;
            if (_isTelegramEnabled == false) return;

            var head = _networks.Count <= 1 ? "" : "[" + server + "] ";
            switch (eventType)
            {
                case EventType.MATCH_ALERT:
                    if (_isDutyAlertEnabled)
                        PostToTelegram(head + GetRouletteName(args[0]) + " >> " + GetInstanceName(args[1]));
                    break;
                case EventType.FATE_BEGIN:
                    if (_telegramSelectedFates.Contains(args[0].ToString()))
                        PostToTelegram(head + GetAreaNameFromFate(args[0]) + " >> " + GetFateName(args[0]));
                    break;
            }
        }

        private void PostToToastWindowsNotificationIfNeeded(string server, EventType eventType, int[] args)
        {
            if (_isToastNotificationEnabled == false) return;
            if (eventType != EventType.FATE_BEGIN && eventType != EventType.MATCH_ALERT) return;

            var head = _networks.Count <= 1 ? "" : "[" + server + "] ";
            switch (eventType)
            {
                case EventType.MATCH_ALERT:
                    if (_isDutyAlertEnabled)
                        ToastWindowNotification(head + GetRouletteName(args[0]), ">> " + GetInstanceName(args[1]));
                    break;
                case EventType.FATE_BEGIN:
                    if (_telegramSelectedFates.Contains(args[0].ToString()))
                        ToastWindowNotification(head + GetAreaNameFromFate(args[0]), ">> " + GetFateName(args[0]));
                    break;
            }
        }

        private void PostToTelegram(string message)
        {
            string chatId = _telegramChatIdTextBox.Text, token = _telegramTokenTextBox.Text;
            if (string.IsNullOrEmpty(chatId) || token == null || token == "") return;

            using (var client = new WebClient())
            {
                client.UploadValues("https://api.telegram.org/bot" + token + "/sendMessage", new NameValueCollection
                {
                    {"chat_id", chatId},
                    {"text", message}
                });
            }
        }

        private void ToastWindowNotification(string title, string message)
        {
            try
            {
                var toast = new Toast(title, message, _networks);
                toast.Show();
            }
            catch (Exception e)
            {
                Logger.Exception(e, "l-toast-notification-error");
            }
        }
        #endregion

        #region Events
        private void CheckBoxTelegram_CheckedChanged(object sender, EventArgs e)
        {
            _isTelegramEnabled = _telegramCheckBox.Checked;
            _telegramChatIdTextBox.Enabled = _isTelegramEnabled;
            _telegramTokenTextBox.Enabled = _isTelegramEnabled;
        }

        private void CheckBoxDutyFinder_CheckedChanged(object sender, EventArgs e)
        {
            _isDutyAlertEnabled = _dutyFinderAlertCheckBox.Checked;
        }

        private void FateTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_lockTreeEvent)
                return;

            _lockTreeEvent = true;
            if (((string)e.Node.Tag).Contains("AREA:"))
            {
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = e.Node.Checked;
                }
            }
            else
            {
                if (e.Node.Checked == false)
                {
                    e.Node.Parent.Checked = false;
                }
                else
                {
                    var flag = true;
                    foreach (TreeNode node in e.Node.Parent.Nodes)
                    {
                        flag &= node.Checked;
                    }

                    e.Node.Parent.Checked = flag;
                }
            }

            _telegramSelectedFates.Clear();

            UpdateSelectedFates(TelegramFateTreeView.Nodes);
            SaveSettings();

            _lockTreeEvent = false;
        }

        private void Network_onReceiveEvent(int pid, EventType eventType, int[] args)
        {
            var server = _networks[pid].Process.MainModule.FileName.Contains("KOREA") ? "KOREA" : "GLOBAL";
            var text = pid + "|" + server + "|" + eventType + "|";
            var pos = 0;
            var isFate = false;

            switch (eventType)
            {
                case EventType.INSTANCE_ENTER:
                case EventType.INSTANCE_EXIT:
                    if (args.Length > 0)
                    {
                        text += GetInstanceName(args[0]) + "|";
                        pos++;
                    }

                    break;
                case EventType.FATE_BEGIN:
                case EventType.FATE_PROGRESS:
                case EventType.FATE_END:
                    isFate = true;
                    text += GetFateName(args[0]) + "|" + GetAreaNameFromFate(args[0]) + "|";
                    pos++;
                    break;
                case EventType.MATCH_BEGIN:
                    text += (MatchType)args[0] + "|";
                    pos++;
                    switch ((MatchType)args[0])
                    {
                        case MatchType.ROULETTE:
                            text += GetRouletteName(args[1]) + "|";
                            pos++;
                            break;
                        case MatchType.SELECTIVE:
                            text += args[1] + "|";
                            pos++;
                            var p = pos;
                            for (var i = p; i < args.Length; i++)
                            {
                                text += GetInstanceName(args[i]) + "|";
                                pos++;
                            }

                            break;
                    }

                    break;
                case EventType.MATCH_END:
                    text += (MatchEndType)args[0] + "|";
                    pos++;
                    break;
                case EventType.MATCH_PROGRESS:
                    text += GetInstanceName(args[0]) + "|";
                    pos++;
                    break;
                case EventType.MATCH_ALERT:
                    text += GetRouletteName(args[0]) + "|";
                    pos++;
                    text += GetInstanceName(args[1]) + "|";
                    pos++;
                    break;
            }

            for (var i = pos; i < args.Length; i++) text += args[i] + "|";

            if (isFate) text += args[0] + "|";

            SendToAct(text);

            PostToToastWindowsNotificationIfNeeded(server, eventType, args);
            PostToTelegramIfNeeded(server, eventType, args);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isPluginEnabled == false)
                return;

            UpdateProcesses();
        }

        private void ToastNotificationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_toadNotificationCheckBox.Checked && !_pluginInitializing)
                ToastWindowNotification(Localization.GetText("ui-toast-notification-test-title"), Localization.GetText("ui-toast-notification-test-message"));
            _isToastNotificationEnabled = _toadNotificationCheckBox.Checked;
        }

        private void ClearLogsButton_Click(object sender, EventArgs e)
        {
            _richTextBox1.Clear();
        }

        private void ActMainFormOnLoad(object sender, EventArgs e)
        {
            _mainFormIsLoaded = true;
            MonitorProcess();
        }

        private void EnableLoggingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_enableLoggingCheckBox.Checked)
            {
                Logger.SetTextBox(null);
                _button1.Enabled = false;
            }
            else
            {
                Logger.SetTextBox(_richTextBox1);
                _button1.Enabled = true;
            }
        }

        private void LanguageComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadData();
            UpdateTranslations();
            LoadFates();
        }
        #endregion

        #region Settings
        private void LoadSettings()
        {
            // All the settings to deserialize
            _xmlSettingsSerializer.AddControlSetting(_languageComboBox.Name, _languageComboBox);
            _xmlSettingsSerializer.AddControlSetting(_toadNotificationCheckBox.Name, _toadNotificationCheckBox);
            _xmlSettingsSerializer.AddControlSetting(_telegramCheckBox.Name, _telegramCheckBox);
            _xmlSettingsSerializer.AddControlSetting(_telegramChatIdTextBox.Name, _telegramChatIdTextBox);
            _xmlSettingsSerializer.AddControlSetting(_telegramTokenTextBox.Name, _telegramTokenTextBox);
            _xmlSettingsSerializer.AddControlSetting(_dutyFinderAlertCheckBox.Name, _dutyFinderAlertCheckBox);
            _xmlSettingsSerializer.AddControlSetting(_enableLoggingCheckBox.Name, _enableLoggingCheckBox);
            _xmlSettingsSerializer.AddStringSetting("_checkedFates");

            if (File.Exists(_settingsFile))
            {
                using (var fileStream = new FileStream(_settingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var xmlTextReader = new XmlTextReader(fileStream))
                {
                    try
                    {
                        while (xmlTextReader.Read())
                        {
                            if (xmlTextReader.NodeType != XmlNodeType.Element)
                                continue;

                            if (xmlTextReader.LocalName == "SettingsSerializer")
                                _xmlSettingsSerializer.ImportFromXml(xmlTextReader);
                        }

                    }
                    catch (Exception ex)
                    {
                        _labelStatus.Text = Localization.GetText("l-settings-load-error", ex.Message);
                    }

                    xmlTextReader.Close();
                }
            }

            _isTelegramEnabled = _telegramCheckBox.Checked;
            _telegramChatIdTextBox.Enabled = _isTelegramEnabled;
            _telegramTokenTextBox.Enabled = _isTelegramEnabled;
            _isDutyAlertEnabled = _dutyFinderAlertCheckBox.Checked;
            _isToastNotificationEnabled = _toadNotificationCheckBox.Checked;
            _selectedLanguage = (Language)_languageComboBox.SelectedItem;
        }

        private void SaveSettings()
        {
            _checkedFates = string.Empty;

            var fatesList = new List<string>();
            foreach (TreeNode area in TelegramFateTreeView.Nodes)
            {
                if (area.Checked)
                    fatesList.Add((string)area.Tag);

                foreach (TreeNode fate in area.Nodes)
                {
                    if (fate.Checked)
                        fatesList.Add((string)fate.Tag);
                }
            }

            _checkedFates = string.Join("|", fatesList);

            using (var fileStream = new FileStream(_settingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var xmlTextWriter = new XmlTextWriter(fileStream, Encoding.UTF8) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' })
            {
                xmlTextWriter.WriteStartDocument(true);
                xmlTextWriter.WriteStartElement("Config"); // <Config>
                xmlTextWriter.WriteStartElement("SettingsSerializer"); // <Config><SettingsSerializer>
                _xmlSettingsSerializer.ExportToXml(xmlTextWriter); // Fill the SettingsSerializer XML
                xmlTextWriter.WriteEndElement(); // </SettingsSerializer>
                xmlTextWriter.WriteEndElement(); // </Config>
                xmlTextWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
                xmlTextWriter.Flush(); // Flush the file buffer to disk
                xmlTextWriter.Close();
            }
        }
        #endregion
    }
}