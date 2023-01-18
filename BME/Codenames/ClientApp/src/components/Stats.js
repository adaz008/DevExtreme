import React, { Component } from "react";
import authService from './api-authorization/AuthorizeService'

export class Stats extends Component {
  static displayName = Stats.name;
  constructor(props) {
    super(props);
    this.state = {
      id: '',
      stats: [],
      loading: true,
    };
  }

  componentDidMount() {
    // this.populateStatsData();
    //console.log(this.state.fields)
  }

  static renderStatsTable(stats) {
    console.log(stats.name)
    return (
      <table className="table table-striped" aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Statisztikák</th>
          </tr>
          <tr>
            <th>Név</th>
            <th>Játszott játékok</th>
            <th>Nyert játékok</th>
            <th>Játszott játékok kémmesterként</th>
            <th>Nyert játékok kémmesterként</th>
          </tr>
        </thead>
        <tbody>
        {stats.map(stats =>
          <tr key={stats.name}>
            <td>{stats.name}</td>
            <td>{stats.gamesPlayed}</td>
            <td>{stats.gamesWon}</td>
            <td>{stats.gamesPlayedAsSpymaster}</td>
            <td>{stats.gamesWonAsSpymaster}</td>
          </tr>
        )}
        </tbody>
      </table>
    );
  }

  onIdChange = (e) => {
    this.setState({ id: e.target.value });

    console.log(this.state.id);
  };

  onIdSubmit = async () => {
    const token = await authService.getAccessToken();
    const response = await fetch(
      `game/user/${this.state.id}`,
      {
        //mode: 'no-cors',
        method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      }
    );

    if(response.status === 404){
      alert("Nincs ilyen felhasználó")
      return
    }
    
    const data = await response.json();
    this.setState({ stats: data, loading: false });
  };


  onRankedSubmit = async () => {
    const token = await authService.getAccessToken();
    const response = await fetch(
      `game/topPlayers`,
      {
        //mode: 'no-cors',
        method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      }
    );

    if(response.status === 404){
      alert("Nincsenek felhasználók")
      return
    }

    const data = await response.json();
    this.setState({ stats: data, loading: false });
  };

  render() {
    let contents = Stats.renderStatsTable(this.state.stats)

    return (
      <div>
        <div className="chooseTeam"><h1 id="tabelLabel">Statisztikák</h1></div>
        <p>Lekérdezés gombra kattintva a szövegdobozba beírt felhasználó adatait kérdezzük le.</p>
        <p>Ranglista gombra kattintva a 10 legtöbb győzelemmel rendelkező felhasználó adatait kérdezzük le.</p>
        <input type="text" onChange={this.onIdChange} />
        <button className="send" onClick={this.onIdSubmit}>Lekérdezés</button>
        <button className="send" onClick={this.onRankedSubmit}>Ranglista</button>
        {contents}
      </div>
    );
  }

  async populateStatsData() {
    const token = await authService.getAccessToken();
    const response = await fetch(
      `https://localhost:7064/game/${this.state.id}`,
      {
        //mode: 'no-cors',
        method: 'GET',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      }
    );
    const data = await response.json();
    console.log(response);
    this.setState({ stats: data, loading: false });
  }
}
