# Klassenbeschreibung

## Views

Entsprechend den Forgaben wurde das komplette Projekt mittels Model-View-View-Model Prinzip umgesetzt. Sprich für jede View wurde ein entsprechendes ViewModel implementiert, welche das komplette handling der eigenen View übernimmt. Somit war es auch nicht nötiog, dass eine View über einen Code-Behind verfügt. Sonderstellung nimmt hierbei lediglich das MainViewModel ein, da dies dafür verantworltich ist, dass bei einem Seitenwechsel das entsprechende ViewModel samt View geladen wird.

### NotifyBase
Um heirbei eine saubere Trennung der einzelnen Views sowie redundante Funktionen zu vermeiden wurde einerseits eine Basis Klasse entwickelt (NotifyBase), welche den übergeordneten Klassen Methoden zur Property Handhabung und einen zugriff zu den Daten ermöglicht. 

### MainView
Weiter haben wir uns dazu entschieden, dass es lediglich ein Window geben wird und wir die unterschiedlichen Views als austauschbare Fragmente in diesem Window handhaben möchten. Hierzu dient die MainView als Grundgerüst. Die MainView stellt das Basisdesign, welches für alle weiteren Views fixer Bestandteil ist, zur Verfügung und bietet noch einen entsprechenden Freiraum für die eigentlichen Views an. Weiter bietet es ebenso die Möglichkeit einer Menüleiste mit der die einzelnen Views entsprechend gewechselt werden können. Diese eigentlichen pages wurden per UserControls umgesetzt und werden im folgenden weiter beschrieben.

### HomeView
Die HomeView ist die eigentliche Startseite der Applikation und bietet neben einigen einführenden Worten die Möglichkeit an Daten aus dem Web oder von einem lokalen .json File zu beziehen. Weiter besteht natürlich ebenfalls die Möglichkeit bezogene Daten in einem lokalen .json File abzuspeichern. Ebenfalls besteht die Möglichkeit alle Animationen der Charts zu aktivieren oder zu deaktivieren.

### CountryStatsView
Die Country Statsistics View erlaubt es dem Nutzer nun detailierte Informationen für ein beliebiges geladenes Land in graphisch aufbereiteter Form zu begutachten. Als besonderheit wird dem Nutzer heir gleich die "Glockenkurve", welche in den Medien immer wieder diskutiert wurde, aufbereitet. Weiter werden dem Nutzer noch einige Einstellungen zur Verfügung gestellt um die dargebotene Statistik anzupassen.

### CountryComparisonView
Die Country Comparison View erlaubt es dem Nutzer nun mehrere Länder miteindaer vergleichen zu können. Um einen Vergleich zu ermöglichen können mehrere Länder ausgewählt werden und weiter noch die Entsprechende Kennzahl mit der der Vergleich passieren soll. Ebenso ist es möglich die Zeitspanne inder der Vergleich stattfindet zu spezifizieren sowie einige andere Einstellungen zu tätigen um die dargebotene Statistik anzupassen.

### WorldMapView
Die World Map bietet dem Nutzer einen Überblick über die einzelnen Länder einer dediziert ausgewählte Kennzahl dargestellt in einer Weltkarte. Hierbei erfolgt der Vergleich tageweise und der Vergleichstag kann über einen entsprechenden Slider ausgewählt werden. Wird ein Land ausgewählt, so erscheinen die detialierten Informationen über dieses Land mit allen verfügbaren Kennzahlen zu diesem Tag. Per Defualt werdn diese entsprechend dem Globalen Ergebnis dargestellt.

### DataListView
Die Data List View bietet abschließend noch eine übersicht über alle geladenen Datensätze an. Hierzu wird ein DataGrid eingesetzt um die Daten entsprechend den Ländern zuweisen zu können. Das komplette Datagrid kann in aufsteigender oder absteigender Reihenfolge sortiert werden.

### Klassendiagramm
Abschlißend folgt noch ein kleines Klassendiagramm, welches die zuvor beschriebenen funktionalen Anforderungen und zusammenhänge der einzelnen Klassen noch verdeutlicht:
 
![View Struktur](ViewStructure.png)

## Charts

Damit die Diagramme nicht selbst gezeichnet werden müssen, wurde eine externe Bibliothek dazu verwendet.
Die Wahl fiel dabei auf die Bibliothek LiveCharts, da diese die für uns relevanten Diagrammtypen zur Verfügung stellt.

Folgende Diagramme werden dargestellt:

1. Ein Balkendiagramm zur Darstellung eines Datensatzes über die Zeit.
2. Ein horizontales Balkendiagramm zur Darstellung von Verhältnissen.
3. Eine Weltkarte als Heatmap.

Jeder Diagrammtyp wurde in einem eigenem `UserControl` verwirklicht, um die Schnittstelle zu den Bibliothekstypen
anzupassen und eventuelle Unzulänglichkeiten ausmerzen zu können.

Die `UserControl`s `TimelineChart`, `RatioBar` und `BasicGeoMap` implementieren diese Möglichkeiten.
Neben der Datenschnittstelle stellen `TimelineChart` und `RatioBar` auch die Möglichkeit bereit, die Diagramme zu
animieren und, im Falle von `TimelineChart`, auch zwischen einigen wenigen Darstellungsarten zu wählen und die
Achsenskalierung der Y-Achse zwischen linear und logarithmisch zu ändern.

All diese Funktionen wurden per Dependency-Properties implementiert, damit diese in den Views auch per Data-Binding
angesteuert werden können.

![View Struktur](ChartStructure.png)


## Model

![Model Struktur](ModelStructure.png)
