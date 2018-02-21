namespace PushMessagesSender
{
    /// <summary>
    /// Общие настройки Push-уведомлений
    /// </summary>
    /// Текущие варианты конфигурации:
    /// Уведомления о новостях и грузах
    /// Уведомления только о новостях
    /// Уведомления только о грузах
    /// Уведомления отключены
    public enum PushSettings
    {
        NewsAndCargo,
        NewsOnly,
        CargoOnly,
        Disabled
    }
}