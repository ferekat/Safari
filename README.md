# 🦁 Safari Tycoon – Szafari Park Szimulátor

A **Safari Tycoon** egy egyjátékos, tycoon jellegű, valós idejű park szimulációs játék, ahol a játékos egy afrikai szafari park igazgatójaként irányítja a terület működését, gazdaságát és élővilágát.

A cél egy stabilan működő, nyereséges és ökológiailag fenntartható szafari park fenntartása.

---
https://github.com/user-attachments/assets/9c6bfcc6-8968-4c19-8fce-0938a420157b

## 🎮 Játékmenet alapjai

- 2D, **felülnézetes pálya**
- Valós idejű szimuláció
- Az idő gyorsítható:
  - ⏱️ Óra
  - 📅 Nap
  - 🗓️ Hét
- Legalább **3 nehézségi szint**

A pálya rácsalapú logikára épül (objektumok rácspontokra helyezhetők), de az állatok és járművek mozgása vizuálisan folyamatos.

---

## 🌿 Környezet és terep

A pályán természetes módon és a játékos által is elhelyezhetők:

### Növényzet
- Bokrok  
- Fák  
- Füves területek  
- Legalább **3 telepíthető növénytípus**

### Víz
- Kezdeti vízlelőhelyek  
- Építhető tavak  

A növények és vízforrások kulcsfontosságúak az állatok túléléséhez.

---

## 🐘 Állatok

A szafariban **szabadon élő állatpopulációk** találhatók.

### Típusok
- 🐾 Növényevők (legalább 2 faj)
- 🦁 Ragadozók (legalább 2 faj)

### Viselkedés
- A növényevők növényeket fogyasztanak  
- A ragadozók növényevőkre vadásznak  
- Minden állatnak szüksége van vízre  
- Az állatok:
  - Öregszenek  
  - Egyre többet esznek  
  - Korlátozott ideig élnek  

### Csoportos viselkedés
- Saját fajuk csoportjában élnek és vándorolnak  
- Felnőtt egyedeket tartalmazó csoportok szaporodhatnak  

### Mozgás és döntéshozatal
- Jóllakott állatok pihennek  
- Ezután heurisztika (részben véletlenszerű) alapján új célpontot választanak  
- Éhes/szomjas állatok a már ismert táplálék- vagy vízforrásokhoz mennek  

---

## 🚙 Turisták és dzsipek

- A turisták dzsippet bérelnek a park bejárásához  
- Egy dzsipp **max. 4 utast** szállít  
- A dzsippeket a játékos vásárolja  

### Útvonalak
- Van egy **bejárat** és egy **kijárat**
- A játékosnak utakat kell építenie
- A dzsipek:
  - Véletlenszerű útvonalon viszik a turistákat bejárattól kijáratig  
  - Visszafelé utasok nélkül térnek vissza  

---

## 💰 Gazdaság

A játékos kezdőtőkével indul.

### Kiadások
- Növények
- Állatok
- Dzsipek
- Utak
- Egyéb eszközök

### Bevételek
- Állatok eladása  
- Turisták (dzsipp bérlés)

A turisták száma függ:
- A belépődíjtól  
- A látott állatok számától és változatosságától  

---

## 🏁 Játék vége

### Győzelem
A játékos nyer, ha a nehézségtől függően **3 / 6 / 12 egymást követő hónapban**:

- A látogatók száma egy küszöb felett marad  
- A növényevő állatok száma küszöb felett marad  
- A ragadozók száma küszöb felett marad  
- A tőke egy küszöb felett marad  

### Vereség
Azonnali vereség, ha:
- A park csődbe megy  
- Kipusztul az összes állat  

---

## 🧩 Választható részfeladatok

A projekt bővíthető az alábbi funkciókkal:

### 🗺️ Minimap
- Nagyobb pálya a látható területnél  
- Görgethető nézet  
- Navigálható minimap  

### 🔫 Orvvadászok
- Állatokat ölnek vagy elrabolnak  
- Csak turisták vagy vadőrök közelében látszanak  

### 🛡️ Vadőrök
- Kijelölhető célpont ragadozók ellen  
- Fizetést igényelnek  
- Védenek az orvvadászok ellen  

### 🎯 Irányítható vadőrök
- Konkrét orvvadászokra küldhetők  
- Fejpénz jár az elfogásukért  
- Az orvvadászok visszatámadhatnak  

### ⛰️ Terepi akadályok
- Dombok és folyók  
- Lassítják a mozgást  
- A dombokról messzebbre látnak az állatok  
- A folyók ivóvízforrások is  

### 💾 Perzisztencia
- Játékállapot mentése és betöltése  
- A mozgásban lévő élőlények onnan folytatják, ahol a mentés történt  

---

## 🎯 A projekt célja

Egy komplex, dinamikus ökoszisztémát és gazdasági rendszert szimuláló játék létrehozása, ahol a játékos döntései közvetlen hatással vannak:

- Az állatpopulációkra  
- A turisták elégedettségére  
- A park pénzügyi stabilitására  

**Safari Tycoon – Menedzseld a vadont. 🌍**
