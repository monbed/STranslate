using STranslate.Plugin;

namespace STranslate.Core;

public class ServiceSettings
{
    private AppStorage<ServiceSettings> Storage { get; set; } = null!;

    public string ReplaceSvcID { get; set; } = string.Empty;
    public string ImageTranslateSvcID { get; set; } = string.Empty;
    public List<ServiceData> TranSvcDatas { get; set; } = [];
    public List<ServiceData> TtsSvcDatas { get; set; } = [];
    public List<ServiceData> OcrSvcDatas { get; set; } = [];
    public List<ServiceData> VocabularySvcDatas { get; set; } = [];

    public void SetStorage(AppStorage<ServiceSettings> storage) => Storage = storage;

    public void Initialize() { }

    public void Save() => Storage.Save();
}

public class ServiceData
{
    public string SvcID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public ExecutionMode ExecMode { get; set; } = ExecutionMode.Automatic;
    public bool AutoBackTranslation { get; set; } = false;
    public ServiceData() { }
    public ServiceData(string svcId, string name, bool isEnabled, ExecutionMode execMode = ExecutionMode.Automatic, bool autoBackTranslation = false)
    {
        SvcID = svcId;
        Name = name;
        IsEnabled = isEnabled;
        ExecMode = execMode;
        AutoBackTranslation = autoBackTranslation;
    }
}

public enum ServiceType
{
    Translation,
    OCR,
    TTS,
    Vocabulary,
}
