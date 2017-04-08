
	Fastwel NIM351 BSP for Linux 2.6.xx
	===================================

Version 1.00
NIM351 v1.2


	This pack contains drivers and examples for Fastwel NIM351 card.
It was tested on 2.6.30 kernel and above. The following is available:

- CAN driver and tests (socketcan.tgz). See socketcan project:
  http://developer.berlios.de/projects/socketcan/.
- serial patch for the 2.6.32 and 2.6.35 kernels.

	A serial port patch adds automatic RS-485 direction control.
Port configuration can be set by setserial, this is an example of the
configuration entries in serial.conf:

	/dev/ttyS2 uart 16850 port 0x100 irq 6 baud_base 3686400
	/dev/ttyS3 uart 16850 port 0x108 irq 7 baud_base 3686400


	All examples have a makefile so they can be built with make.
The drivers have build scripts. Before compiling drivers make sure
that in /usr/src/linux there are kernel source already compiled with
necessary config.
	The recommended compiler is gcc 4.4.5 or newer.


(c) 2010-2011 Fastwel Co, Ltd. All rights reserved.
