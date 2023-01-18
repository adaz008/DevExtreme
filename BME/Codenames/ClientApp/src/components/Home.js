import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
        <h1>Hello, játékosok!</h1>
        <p>Üdvözlünk a fedőnevek online játszható játékában</p>
        <p><strong>Szabályzat:</strong></p>
        <ul>
          <li><strong>Előkészületek:</strong>
            <ul>
              <li>A játékosoknak két csapatba kell osztaniuk magukat.</li>
              <li>Mindkét csapatban legalább 2embernek kell lennie</li>
              <li>Mindkét csapatnak kell egy kémfőnököt választani.</li>
            </ul>
            </li>
          <li><strong>Játék menete:</strong>
            <ul>
              <li>Kezdő csapat véletlenszerűen lesz kiválasztva.</li>
              <li>Kémfőnök nem válthat csapatot, a többiek a játék indítása előtt még átmehetnek a másik csapatba.</li>
              <li>Kezdő csapatnak 9, az ellenfél csapatának 8 szót kell kitalálnia.</li>
              <li>Kémfőnök megad egy kulcsszót és egy számot, amely alapján a csapatának rá kell jönnie, melyik szavakra gondolt.</li>
              <li>A kémfőnök által megadott szám jelzi az adott körben kitalálandó szavak számát és maximum ennyit lehet tippelni.</li>
              <li>Ha a tippek száma elfogyott, akkor a csapat köre befejeződik.</li>
              <li>A pályán található 7 ártatlan járókelő illetve egy bérgyilkos a piros és kék színű ügynökök mellett.</li>
              <li>Ha a kémek a saját csapatukhoz tartozó kártyára kattintanak, akkor a mező a csapat színére változik, amennyiben ez nem az utolsó tipp volt tovább tippelhetnek de nem kapnak új kulcsszót</li>
              <li>Ha a kémek egy ártatlan járókelő kártyára kattintanak, akkor a mező fehér színűre változik.Ezzel a csapat köre befejeződik.</li>
              <li>Ha a kémek a másik csapathoz tartozó kártyára kattintanak, akkor a mező az ellenfél csapat színére változik.Ezzel a csapat köre befejeződik.</li>
              <li>Ha a kémek a bérgyilkos kártyára kattintanak, akkor a játék véget ér és az ellenfél csapata nyert.</li>
            </ul>
          </li>
          <li><strong>Játék vége:</strong>
            <ul>
              <li>Ha valamelyik csapat összes kártyája felfordításra kerül, akkor megnyerte a játékot.</li>
              <li>Ha valamelyik csapat bérgyilkosra kattintott, akkor a másik csapat nyer.</li>
            </ul>
          </li>
        </ul>
      </div>
    );
  }
}
