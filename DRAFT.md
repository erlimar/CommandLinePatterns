# Suite de testes

## CommandLineApp mínima

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.PrintUsage();

// cmd - My Command Line App
// 
// USAGE:
//     $ cmd [<options>|<flags>] [<args>]
```

## CommandLineApp com versão

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Version("1.0.0-alpha-67890");

cmdApp.PrintUsage();

// cmd - My Command Line App [version 1.0.0-alpha-67890]
// 
// USAGE:
//     $ cmd [<options>|<flags>] [<args>]
```

## CommandLineApp com direitos autorais

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Copyright("(c) 1991-%{YEAR} E5R Development Team. All rights reserved.");

cmdApp.PrintUsage();

// cmd - My Command Line App
// Copyright (c) 1991-2018 E5R Development Team. All rights reserved.
// 
// USAGE:
//     $ cmd [<options>|<flags>] [<args>]
```

## CommandLineApp com sinopse

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Synopsis(new string[]
{
    "This is a my command line app to exemplify the use of this",
    "exceptional library. In the next few lines you'll see how",
    "easy it is to write a command line program."
});

cmdApp.PrintHelp();

// cmd - My Command Line App
// 
// This is a my command line app to exemplify the use of this
// exceptional library. In the next few lines you'll see how
// easy it is to write a command line program.
// 
// USAGE:
//     $ cmd [<options>|<flags>] [<args>]
```

## CommandLineApp mais completo

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App")

    .Version("1.0.0-alpha-67890");
    .Copyright("(c) 1991-%{YEAR} E5R Development Team. All rights reserved.");
    .Synopsis(new string[] {
        "This is a my command line app to exemplify the use of this",
        "exceptional library. In the next few lines you'll see how",
        "easy it is to write a command line program." });

cmdApp.PrintUsage();

// cmd - My Command Line App [version 1.0.0-alpha-67890]
// Copyright (c) 1991-2018 E5R Development Team. All rights reserved.
// 
// USAGE:
//     $ cmd [<options>|<flags>] [<args>]

cmdApp.PrintHelp();

// cmd - My Command Line App [version 1.0.0-alpha-67890]
// Copyright (c) 1991-2018 E5R Development Team. All rights reserved.
// 
// This is a my command line app to exemplify the use of this
// exceptional library. In the next few lines you'll see how
// easy it is to write a command line program.
// 
// USAGE:
//     $ cmd [<options>|<flags>] [<args>]
```

## Definindo e usando opções

```csharp
var cmdApp = new CommandLineApp("cmd", "My Command Line App");

cmdApp.Option("Action", "The app action")
      .Template("-a|--action");

var args = new string[] { "-a", "My Action Value" };

CommandLineOptions options = cmdApp.Parse(args);

Assert.True(options.HasValue("Action"));
Assert.Equal(options.ValueOf("Action"), "My Action Value");
```