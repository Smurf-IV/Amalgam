# Amalgam

Synonyms: alloy, amalgamation, blend, combination, combo, fusion

This will make an FTP target appear as a source drive within the windows OS (i.e. in Explorer or a DOS box). It will allow Media playback, file updates, backup programs etc. to function as if they are talking to networked shared drive.
OS Requirements:

This uses the .Net4 x32 Full profile, so please install that.
You will also need to install Dokan for the OS you are using.
FAQs

Q Aren't there other (free) offerings that do this?
A Yes there are, but I was not able to find one that worked reliably and at the full potential of both read and write speeds in Win 7 and above.

Q Why do this?
A I wanted a drive that took an FTP target and placed it into windows as a drive in explorer (and dos); so that Media players could extract data and play large 1080p files without jitter.

Q Why C#?
A This is so that any memory that is being used is tidied up cleanly by the OS, and to make any interface pretty via the use of WCF (later on :-))

Q Hasn't Dokan got a few issues?
A Yes, but these are mainly to do with security access, and mapped file access.
A2 By (currently) setting the Dokan driver to be a network drive, this allows a lot of the problem applications to work (e.g. notepad is the quickest to show these problems)

Q What about WebDav?
A I suppose that this could be extended to perform such functions, but currently is beyond the scope of this first phase.

Q What about SFTP / FTPS ?
A The StarkSoftFTP library does have this functionality, and it could be used later on - any volunteers to take that part on ?
