ChangeLog:

## Version 2.5.0
 * ksp 1.8-1.9
 * CommNetAntennasConsumptor:
    * supported DMagic Orbiral Science: Soil Moisture Sensor and SIGINT
    * added ModuleAntennaToggler - module to disable/enable every "static" (unretractable) antenna. 
      So now you can disable static antennas, making them isn't consuming EC, and a vessel will lose its antenna power.
      Also if you disable antenna at launch, it will be autoenabled on transmission request and later autodisabled
      (same as retractable antennas autodeplyed and autoretracted)
 * added Bandwidth field to PAW

## Version 2.3.5
 * backport 2.4.1 to ksp 1.8

## Version 2.4.1
 * Packet size and cost to PAW 
 * fix DSN power info

## Version 2.4.0
 * ksp 1.9.0
 * CommNetAntennasConsumptor:
     * disabled consumption for the INTERNAL antennas
     * DIRECT consumption reduced by 2.5 times.

## Version 2.3.4
 * Hide actions for CommNetAntennasConsumptor

## Version 2.3.3
 * Fix nulrefs

## Version 2.3.2
 * Fix not-extendable antennas

## Version 2.3.1
 * Updated translation

## Version 2.3.0
 * support for NF Exploration
 * new Extras - CommNetAntennasConsumptor on CKAN - enable ModuleGeneratorAntenna, 
   which makes antennas consume EC for supporting CommNet (always, when extended)
 * recompiled for ksp 1.8.1
 * updated MM 4.1.3

## Version 2.2.0
 * recompiled for ksp 1.8.0
 * showed "Consumption" as EC/Mit
 * hided "Antenna State" for Internal Antennas, because they can't transmit science
 * supported parts with several ModuleDataTransmitters
 * updated MM 4.1.0
 * targeted .NET 4.7.2

## Version 2.1.2
 * remove unused patch filter

## Version 2.1.1
 * PAW: fix default rating
 * PAW: compact for internal antenna, 
 * update MM 4.0.3

## Version 2.1.0
 * Antenna Type and CombinabilityExponent in the PAW (Part-Action-Window)
   It show up only with enabled Advanced Tweakable setting 
 * added MM v4.0.2
 * recompiled for ksp 1.7.0
 * targeted .NET 3.5 
 

## Version 2.0.0
 * This plugin was exluded from the "CommNet Antennas Extension"
   Salute the "CommNet Antennas Info"
 * recompiled for ksp 1.6.0



CommNet Antennas Extension ChangeLog:

## Version 1.1.1.1
 * Plugin: Localized Internal AntennaType

## Version 1.1.1
 * Plugin: Localized AntennaType

## Version 1.1.0 (Plugin Update)
 * shows modified ratings (if you have changed power modifiers in the settings)
 * shows all DSN levels (Custom Barn Kit supported)
 * shows DSN ratings, hightlights active DSN level, 
 * shows range to Built-in antenna for relays
 * compact version for internal antennas

## Version 1.0.3
 * HG-25 is HG-32 now.
   When this antenna was included, the purpose was to make 
   InnerSOI Commnet Network reaching Minmus with 2 of those on each vessel.
   But HG-25 doesn't make this true, because of Combinability, 
   so the antenna-power was increased.

## Version 1.0.2
 * fixed large power consumption of the HG-25

## Version 1.0.1
 * Recompile for KSP 1.5.1
 * fixed RelayTech One antennaType (Relay)

## Version 1.0.0
 * Recompile for KSP 1.5.0
 * Russian localization
 * Update MM to 3.1.0

## Version 0.9.5
 * check working with RemoteTechRedevAntennas 0.1.1
 * update desc
 * add MM to archive

## Version 0.9.3
 * fix bug in the ja-localization

## Version 0.9.2
 * fix technodes

## Version 0.9.1
 * remove RT art assets

## Version 0.9
 * direct antennas: 5M and 500G
 * relay antennas: 25M, 7G, 25G, 500G
 * plugin for showing CombinabilityExponent in the VAB
