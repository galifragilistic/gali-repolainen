using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using BatchInspector.Model;
using System.Text;
using System.Linq;
using System.IO;
using System.Windows.Input;
using Ionic.Zip;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System.Xml;
using System;

namespace BatchInspector.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public ICommand CheckZipsCommand { get; private set; }

        private string _textBoxText;
        public string TextBoxText
        {
            get
            {
                return _textBoxText;
            }
            set
            {
                if (_textBoxText != value)
                {
                    _textBoxText = value;
                    RaisePropertyChanged("TextBoxText");
                }
            }
        }
        private string[] _zipPaths;
        public string[] ZipPaths
        {
            get
            {
                return _zipPaths;
            }
            set
            {
                if (_zipPaths != value)
                {
                    _zipPaths = value;
                    RaisePropertyChanged("ZipPaths");
                }
            }
        }



        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            TextBoxText = "Drag and drop batch files here.";
            Messenger.Default.Register<string[]>(this, WriteZipNames);

            CheckZipsCommand = new RelayCommand(ExecuteCheckZipsCommand);
        }

        private void ExecuteCheckZipsCommand()
        {
            if (ZipPaths == null) return;
            // check zips for errors
            var sb = new StringBuilder();
            foreach (string zip in ZipPaths)
            {
                sb.Append(Path.GetFileName(zip) + ":\n");
                string[] errors = Validate(zip);
                if (errors.Length < 1) { sb.Append("\tNo errors detected.\n\n"); continue; }
                for (int i = 0; i < errors.Length; i++)
                    sb.Append("\t" + errors[i] + "\n");
                sb.Append("\n");
            }
            TextBoxText = sb.ToString();
        }

        private void WriteZipNames(string[] files)
        {
            var zipPaths = new List<string>();
            var sb = new StringBuilder();
            foreach (string f in files)
            {
                var ext = Path.GetExtension(f);
                if (ext.Equals(".zip", System.StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(f + "\n");
                    zipPaths.Add(f);
                }
            }
            TextBoxText = sb.ToString();
            ZipPaths = zipPaths.ToArray();
        }

        private string[] Validate(string filePath)
        {
            var errors = new List<string>();
            var tempFolder = AppDomain.CurrentDomain.BaseDirectory + "\\zipValidation";// @"C:/Temp/zipValidation";
            string mainFile = "";
            try
            {
                if (Path.GetFileName(filePath).Contains(" ")) errors.Add("Filename contains spaces");
                if (Path.GetFileName(filePath).Contains(",")) errors.Add("Filename contains commas");
                ExtractXMLFilesTo(filePath, tempFolder);
                try
                {
                    mainFile = Directory.GetFiles(tempFolder).Single(path => Path.GetFileNameWithoutExtension(path) == Path.GetFileNameWithoutExtension(filePath));
                }
                catch (Exception)
                {
                    errors.Add("No main file found.");
                    return errors.ToArray();
                }
                var nestFiles = new List<string>();
                // find the nest files (where document element is <Nest>)
                foreach (string file in Directory.GetFiles(tempFolder).Where(path => !path.Equals(mainFile)))
                {
                    using (var xr = XmlReader.Create(file))
                    {
                        while (!xr.EOF)
                        {
                            if (xr.NodeType == XmlNodeType.Element && xr.Depth == 0)
                            {
                                if (xr.Name == "Nest")
                                {
                                    nestFiles.Add(file);
                                    break;
                                }
                                if (xr.Name == "Batch")
                                    break;
                            }
                            xr.Read();
                        }
                    }
                }

                // check the number of nest files announced in the main file
                using (var xr = XmlReader.Create(mainFile))
                {
                    var announcedNests = new List<string>();
                    while (!xr.EOF)
                    {
                        if (xr.NodeType == XmlNodeType.Element && xr.Name == "NestXMLFileName")
                        {
                            xr.Read();
                            announcedNests.Add(xr.Value);
                        }
                        xr.Read();
                    }
                    if (announcedNests.Count() != nestFiles.Count())
                        errors.Add("Different amounts of nest files than is announced. Amount announced: " + announcedNests.Count() + ", amount of nest files found: " + nestFiles.Count());
                    // check if announced files are found
                    foreach (string file in announcedNests)
                    {
                        if (!nestFiles.Any(f => Path.GetFileName(f).Equals(file)))
                            errors.Add("Nest file '" + file + "' is announced but not found.");
                    }
                }

                string currentFileName = "";
                bool foundTimes, foundLastPart, flowIDzero;
                foreach (string file in nestFiles)
                {
                    foundTimes = false;
                    foundLastPart = false;
                    flowIDzero = false;
                    currentFileName = Path.GetFileName(file);
                    using (var xr = XmlReader.Create(file))
                    {
                        var lastPartId = "";
                        var lastPartName = "";
                        while (!xr.EOF) {
                            if (xr.NodeType == XmlNodeType.Element) {
                                if (xr.Name == "BatchName") {
                                    xr.Read();
                                    if (xr.Value != Path.GetFileNameWithoutExtension(mainFile))
                                        errors.Add(currentFileName + ": Batch name incorrect: \"" + xr.Value + "\"");
                                }

                                if (xr.Name == "NestName") {
                                    xr.Read();
                                    if (xr.Value != Path.GetFileNameWithoutExtension(file))
                                        errors.Add(currentFileName + ": Nest name is different from the file name: \"" + xr.Value + "\"");
                                }

                                if (xr.Name == "Part") {
                                    if (xr.AttributeCount == 1) lastPartId = xr.GetAttribute(0);
                                }

                                if (xr.Name == "PartName") {
                                    xr.Read();
                                    lastPartName = xr.Value;
                                }
                                //// check for allowed addresses for scraps
                                //if (xr.Name == "Address")
                                //    if (lastPartName.Contains("SCRAP")) {
                                //        var scrapAddresses = new[] { "0", "1", "291", "292", "691", "692" };
                                //        xr.Read();
                                //        if (!(scrapAddresses.Any(a => a == xr.Value)))
                                //            errors.Add(currentFileName + ": Invalid scrap address: " + xr.Value);
                                //    }
                                //// check if scrap sorting type is not 0 when address is > 600
                                //if (xr.Name == "SortingType") {
                                //    if (lastPartName.Contains("SCRAP")) {
                                //        xr.Read();
                                //        if (xr.Value != "0")
                                //            errors.Add(currentFileName + ": Scrap sorting type is " + xr.Value + ", when it should be 0 (ID = " + lastPartId + ")");
                                //    }
                                //}
                                // times
                                if (xr.Name == "Times") {
                                    xr.Read();
                                    foundTimes = true;
                                    decimal sheetLiftTime = 0;
                                    var times = new List<decimal>();
                                    while (!(xr.NodeType == XmlNodeType.EndElement && xr.Name == "Times")) {
                                        // times have to be in ascending order
                                        if (xr.NodeType == XmlNodeType.Element) {
                                            if (xr.GetAttribute("FlowID") == "0") flowIDzero = true;
                                            var attr = xr.GetAttribute("Time"); //xr.AttributeCount == 2 ? xr.GetAttribute(1) : xr.GetAttribute(0);
                                            times.Add(decimal.Parse(attr, System.Globalization.CultureInfo.InvariantCulture));

                                            // store sheet_lift time
                                            if (xr.Name == "SHEET_LIFT") sheetLiftTime = decimal.Parse(attr, System.Globalization.CultureInfo.InvariantCulture);
                                            if (xr.Name == "LAST_PART" || xr.Name == "SHEET_UNLOAD") foundLastPart = true;
                                        }
                                        xr.Read();
                                    }
                                    for (int i = 1; i < times.Count; i++) {
                                        if (times[i] < times[i - 1]) {
                                            errors.Add(currentFileName + ": Times are not in ascending order.");
                                            break;
                                        }
                                    }
                                    if (times.Any(t => t < 0))
                                        errors.Add(currentFileName + ": Negative times.");

                                    if (flowIDzero) errors.Add(currentFileName + ": FlowID zero.");

                                    // SHEET_LIFT time attribute must be at least 20 seconds less than LAST_PART
                                    if (times[times.Count - 1] - sheetLiftTime < 20 && foundLastPart)
                                        errors.Add(currentFileName + ": Sheet lift time is less than 20 seconds from the last cut part\n\t" +
                                            "SHEET_LIFT = " + sheetLiftTime + ", LAST_PART = " + times[times.Count - 1]);
                                }
                            }
                            xr.Read();
                        }
                    }
                    if (!foundTimes)
                        errors.Add(currentFileName + ": <Times> not found");
                    else if (!foundLastPart)
                        errors.Add(currentFileName + ": Neither <SHEET_UNLOAD> nor <LAST_PART> was found");
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
            Directory.Delete(tempFolder, true);
            return errors.ToArray();
        }

        private void ExtractXMLFilesTo(string sourceZip, string destination)
        {
                using (var fs = new FileStream(sourceZip, FileMode.Open))
                {
                    using (ZipFile zip = ZipFile.Read(fs))
                    {
                        foreach (ZipEntry e in zip)
                        {
                            if (Path.GetExtension(e.FileName) != ".xml") continue;
                            if (File.Exists(Path.Combine(destination, e.FileName))) continue;
                            e.Extract(destination, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                }
        }
    }
}