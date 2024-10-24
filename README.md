# CHIP-8 Emulator (C#)

Это эмулятор CHIP-8, написанный на C#. Проект создан по мотивам статьи [Guide to making a CHIP-8 emulator](https://tobiasvl.github.io/blog/write-a-chip-8-emulator/) от Tobias V. Langhoff.

Для работы с графикой, звуком и клавиатурой в проекте используется библиотека [Raylib-CSharp](https://github.com/MrScautHD/Raylib-CSharp).

## Описание проекта

CHIP-8 — это простой интерпретируемый язык программирования, разработанный в 1970-х годах. Эмулятор в этом проекте выполняет интерпретацию программ, написанных для этой виртуальной машины, и визуализирует их через графический интерфейс.

## Зависимости

Перед началом убедитесь, что у вас установлены следующие компоненты:
* .NET SDK (не ниже версии 6.0)
* [Raylib-CSharp](https://github.com/MrScautHD/Raylib-CSharp) (для работы с графикой и звуком)

### Установка Raylib-CSharp

Чтобы установить Raylib-CSharp, выполните команду:

```bash
dotnet add package Raylib-cs --version <latest_version>
```

## Сборка проекта
Вы можете собрать проект для разных платформ с помощью .NET CLI.

### Windows
Для сборки под Windows выполните следующую команду:

```bash
dotnet publish -r win-x64 -c Release
```

### Linux
Для сборки под Linux выполните следующую команду:

```bash
dotnet publish -r linux-x64 -c Release
```

## Запуск эмулятора
После успешной сборки можно запустить эмулятор, перейдя в папку publish и выполнив соответствующий исполняемый файл для вашей системы:

```bash
./chip8-emulator.exe
```

## Управление
* Графика: 64x32 пикселя, чёрно-белый экран.
* Клавиатура: Поддержка 16-клавишной раскладки, соответствующей оригинальному набору CHIP-8.
* Звук: Воспроизведение звукового сигнала при активации таймера.

## Источники и дополнительная информация
* [Официальная документация CHIP-8](https://en.wikipedia.org/wiki/CHIP-8)
* [Raylib Documentation](https://www.raylib.com/cheatsheet/cheatsheet.html)

## Лицензия
Этот проект распространяется под лицензией MIT. Подробности можно найти в файле LICENSE.
