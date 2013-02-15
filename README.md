RelayCard
=========

A quick hack Mono/C# lib/utility, to control a conrad 8-channel relay card. 
...to be continued.

![Raspberry Pi controls 8-port relay card](http://www.kripp.ch/drop/public/raspb/20130215_221619.jpg "Raspberry Pi controls 8-port relay card")

(card: http://www.conrad.ch/ce/de/product/197730/C-Control-8fach-Relaiskarte-230-VAC-16-A-Baustein-8-Relaisausgaenge)

Todo: 
  - Refactor
  - Test with more than one card & Implement Broadcasts
  - Verify card init methods
  
see [Conrad8RelayCard.cs](https://github.com/krippendorf/RelayCard/blob/master/ch.kripp.RelayCard/ch.kripp.RelayCard/Conrad8RelayCard.cs) and send your pull requests ;)

Samples: 
<pre>
-> % mono ch.kripp.ConradRelayCardUtil.exe -dc SETPORT -m '1;10000001' -p /dev/ttyUSB0 
DEBUG - Setting card port to /dev/ttyUSB0
DEBUG - Initializing card(s)
INFO - Found (1) card(s)
DEBUG - Command: SetPort
DEBUG - SetPort address param is: 1, data byte: 129
INFO - Result frame: [ADR:1 DAT:129, CRC:124]
</pre>

<pre>
-> % mono ch.kripp.ConradRelayCardUtil.exe -dnc GETPORT -m '1' -p /dev/ttyUSB0
DEBUG - Setting card port to /dev/ttyUSB0
DEBUG - Option 'DoNotInit'has been set, card not initialized
DEBUG - Command: GetPort
DEBUG - GetPort address param is: 1
INFO - Result frame: [ADR:1 DAT:129, CRC:125]
INFO - Result state: [byte:129] mask:10000001
</pre>

