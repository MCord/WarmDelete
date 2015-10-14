# Warm Delete

Warm delete is a windows command line tool to remove files and folders that are in use. 


### arguments

```
WarmDelete 1.1.0.0
Copyright c  2015

  -t, --target     Required. Target file or folder.

  -v, --verbose    (Default: False) Prints detailed messages to standard
                   output.

  --no-kill        (Default: False) Do not kill the target process.

  --no-message     (Default: False) Do not send a close message (usefull when
                   the process has no visible windows).

  --timeout        (Default: 60) How many seconds to wait for when stopping a
                   service.

  --help           Display this help screen.
```

### Thanks

This project uses source code from 

* [Command Line Parser](https://commandline.codeplex.com/)
* [This stack overflow question](http://stackoverflow.com/a/3346055/176942)
* [WyUpdater](https://code.google.com/p/wyupdate/)

### Latest build status
[![build status](https://ci.appveyor.com/api/projects/status/81pchvaj9baq2qom/branch/master?svg=true)](https://github.com/MCord/WarmDelete/releases/latest)
