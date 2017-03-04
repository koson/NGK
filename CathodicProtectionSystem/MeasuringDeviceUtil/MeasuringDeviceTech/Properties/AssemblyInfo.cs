using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Управление общими сведениями о сборке осуществляется с помощью 
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("NGKDevicesTerminal")]
[assembly: AssemblyDescription("Технологическая программа для работы с устройствами НГК-КИП")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ООО " + AssemblyHelper.quote + "НПО " + AssemblyHelper.quote + "Нефтегазкомплекс-ЭХЗ" + AssemblyHelper.quote)]
[assembly: AssemblyProduct("Терминал для рабокты с НГК-КИП")]
[assembly: AssemblyCopyright("Copyright © 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Параметр ComVisible со значением FALSE делает типы в сборке невидимыми 
// для COM-компонентов.  Если требуется обратиться к типу в этой сборке через 
// COM, задайте атрибуту ComVisible значение TRUE для этого типа.
[assembly: ComVisible(false)]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("7ddfa9f7-bb39-4ad8-ae44-997156d0bfa9")]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//      Основной номер версии
//      Дополнительный номер версии 
//      Номер построения
//      Редакция
//
// Можно задать все значения или принять номер построения и номер редакции по умолчанию, 
// используя "*", как показано ниже:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.7")]
[assembly: AssemblyFileVersion("1.0.0.7")]

public static class AssemblyHelper
{
    public const string quote = "\u0022";
} 