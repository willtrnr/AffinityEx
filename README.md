AfinityEx
=========

Primitive plugin injection system for Serif Affinity products.

Greatly inspired by [BepInEx](https://github.com/BepInEx/BepInEx) and from
which the name is derived. [Harmony](https://github.com/pardeike/Harmony) is
used to hook into the original application code to wire plugins in relevant
places.

This repository includes 2 proof of concept plugins demonstrating the
capabilities. One is bundled directly in the `Launcher` project to provide UI
feedback of which plugins are active. The other is in the `Plugins` project and
adds extra keyboard shorcuts to Affinity Designer that made the author's life
better and prompted this whole endeavour.

No precompiled release will be provided at the moment since, while the author
has had good success so far, nothing is fully tested or vetted for daily usage.
You are encouraged to inspect, improve and compile the code yourself.

![](/screenshot.png)

Legal Disclaimer
----------------

This project is licensed under the [MIT license](/LICENSE).

Use at your own risk, the author, contributors and Serif Europe Ltd. cannot be
held liable for corrupted files, loss of work or any other issue that could
arise by using the project and any derived plugin. You recognise that you are
"on your own" and that Serif will be unable to assist you in any way, support
will only be provided by the author and contributors on best effort basis.

This project is not associated in any way with Serif Europe Ltd.

"Serif" and "Affinity" are both registered trademarks of Serif Europe Ltd.