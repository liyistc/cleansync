# ComparisonResult #

> ## Comparison Result contains the result through compare logic, which maintains three different results: Differences on PC, difference on removable devices and lastly the conflict between the two above. ##



---

# Attributes #
### Public ###
  * **public Differences USBDifferences**
    * This is the difference on USB relative to PC, of type Differences.
  * **public Differences PCDifferences**
    * This is the difference on PC relative to USB, of type Differences.
  * **public List`<`Conflicts`>` conflictList**
    * This contains conflicts of PC and USB, which maintained inside a list of Conflicts.


---

# Constructors #

  * **ComparisonResult(Differences USBDifferences, Differences PCDifferences, List`<`Conflicts`>` conflictList)**

---

# Methods #
### Public ###
  * **List`<`string`>` ConvertComparisonResultToListOfString(ComparisonResult comparisonResult)**
### Private ###
  * **string ConflictlistToString(List`<`Conflicts`>` conflictList)**