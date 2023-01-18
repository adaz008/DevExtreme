/* eslint-disable no-undef */
import React, { Component } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import authService from './api-authorization/AuthorizeService'
import { HttpTransportType } from '@microsoft/signalr';
import { LogLevel } from '@microsoft/signalr';
import { Container } from 'reactstrap';
import { textChangeRangeIsUnchanged } from 'typescript';
import './styles.css'




var gameConnection = new HubConnectionBuilder()
    .withUrl('https://localhost:7064/chathub', {
        accessTokenFactory: () => {
            return authService.getAccessToken();
        }, transport: HttpTransportType.WebSockets, skipNegotiation: true
    })
    .configureLogging(LogLevel.Information)
    .build();


export class Game extends Component {
    static displayName = Game.name;

    constructor(props) {
        super(props);
        this.state = {
            fields: [], game: [], user: '', team: -1, redteam: [], blueteam: [], redmaster: false, bluemaster: false, ismaster: false, started: false, clue: '', clueno: 0, cluevalid: false, inputVisible: true,
            redpoints: 0, bluepoints: 0, gameended: -1, chatMessages: [], message: ''
        };

        gameConnection.on('SetPlayers', (red, blue) => {
            this.setState({ redteam: red, blueteam: blue })
            this.forceUpdate();
        })

        gameConnection.on('OperativeEntered', (user, team) => {
            this.setState({ team: team })

        })

        gameConnection.on('SpymasterEntered', (user, team) => {
            if (team === 0) {
                this.setState({ redmaster: true })
            }
            else if (team === 1) {
                this.setState({ bluemaster: true })
            }

        })

        gameConnection.on('StartGame', () => {
            this.setState({
                started: true, gameended: -1
            })
        })
        gameConnection.on('refresh', (f, g) => {

            this.setState({ fields: f, game: g })

        })
        gameConnection.on('IsClueValid', (valid, clue, tips) => {

            this.setState({ cluevalid: valid, inputVisible: false, clue: clue, clueno: tips });
            //this.forceUpdate();
            this.valami();

        })
        gameConnection.on('endGame', (winner) => {
            if (winner === "Red") {
                this.setState({ gameended: 0 })
            }
            else if (winner === "Blue") {
                this.setState({ gameended: 1 })
            }
        })
        gameConnection.on('changeCurrentTeam', () => {
            this.setState({ cluevalid: false, clue: '', clueno: 0 })
        })

        gameConnection.on('ReceiveMessage', message => {

            console.log(this.state.chatMessages.length);
            if (this.state.chatMessages.length > 10)
                this.state.chatMessages.splice(0, 1)
            this.state.chatMessages.push(message);
            this.forceUpdate();
        })

        gameConnection.onclose(async () => {
            await this.startgame();
        })

        this.startgame();
    }


    componentDidMount() {

    }
    valami() {
        if (this.state.cluevalid === false) {
            this.setState({ inputVisible: true });
        }
    }

    getTeam() {
        return this.state.team === 1 ? { backgroundColor: 'rgba(0, 60, 255, 0.438)' } : { backgroundColor: 'rgba(255, 0, 0, 0.438)' }
    }

    renderChat(chatMs) {
        console.log(chatMs)

        return (
            <div>
                <h1>Chat</h1>
                <div className='row'>
                    <div className='column'>
                        <label className='message' htmlFor="message">Üzenet:</label>
                        <input
                            type="text"
                            id="message"
                            name="message"
                            value={this.state.message}
                            onChange={this.onMessageUpdate} />
                    </div>
                    <div className='column'>
                        <button className='send' onClick={this.onSubmitChat}>Küldés</button>
                    </div>
                </div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>Chat</th>
                        </tr>
                    </thead>
                    <tbody>
                        {chatMs.map(chatMs =>
                            <tr key={Math.random() * 1000000 + 1}>
                                <td style={this.getTeam()}>{chatMs}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    setBackground() {
        const style = {
            backgroundImage: "url(/assets/kek_bg.png)",
            height: '100%',
            width: '100%',
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            border: '0px solid',
            padding: '100px',
            borderCollapse: 'collapse'
        }
        const style2 = {
            backgroundImage: "url(/assets/piros_bg.png)",
            height: '100%',
            width: '100%',
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            border: '0px solid',
            padding: '100px',
            borderCollapse: 'collapse'
        }

        if (this.state.team === 1)
            return style
        else
            return style2
    }

    renderGame(fields) {
        return (
            <div>
                <div>
                    <div className="container">
                        <div className="row">
                            <div className="col-sm">
                                <div style={(this.state.cluevalid || !this.state.ismaster || this.state.team != this.state.game[0].curentTeam) ? { display: 'none' } : {}}>
                                    <label htmlFor="message">Kulcsszó:</label>
                                    <input
                                        type="text"
                                        id="clue"
                                        name="clue"
                                        value={this.state.clue}
                                        onChange={this.onClueUpdate} />
                                    <input type="number" id="cards" name="cards" value={this.state.clueno}
                                        min="1" max="9" onChange={this.onCluenoUpdate}></input>
                                    <button className='send' onClick={this.onSubmit}>Elküld</button>
                                </div>
                            </div>
                            <div className="col-sm">
                                <p>Piros csapat pontjai {this.state.game[0].redPoints}</p>
                                <p>Kék csapat pontjai: {this.state.game[0].bluePoints}</p>
                                <p>Hátrelévő tippek száma: {this.state.game[0].remainingTips}</p>
                                <p>Aktuális csapat: {this.state.game[0].curentTeam === 0 ? "Piros" : "Kék"}</p>
                            </div>
                        </div>
                    </div>
                    <table className='table table-striped' aria-labelledby="tabelLabel" style={this.state.cluevalid ? {} : { display: 'none' }}>
                        <thead>
                            <tr>
                                <th>Kulcsszó:</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr key={this.state.clue}>
                                <td>{this.state.clue}</td>
                                <td>{this.state.clueno}</td>
                            </tr>
                        </tbody>
                    </table>
                    <table style={this.setBackground()} className='table' aria-labelledby="tabelLabel">
                        <tbody>
                            <tr>
                                <td><button style={this.returnColor(0)} onClick={() => this.onClick(0)}>{fields[0].fieldPicked ? "" : fields[0].word}</button></td>
                                <td><button style={this.returnColor(1)} onClick={() => this.onClick(1)}>{fields[1].fieldPicked ? "" : fields[1].word}</button></td>
                                <td><button style={this.returnColor(2)} onClick={() => this.onClick(2)}>{fields[2].fieldPicked ? "" : fields[2].word}</button></td>
                                <td><button style={this.returnColor(3)} onClick={() => this.onClick(3)}>{fields[3].fieldPicked ? "" : fields[3].word}</button></td>
                                <td><button style={this.returnColor(4)} onClick={() => this.onClick(4)}>{fields[4].fieldPicked ? "" : fields[4].word}</button></td>
                            </tr>
                            <tr>
                                <td><button style={this.returnColor(5)} onClick={() => this.onClick(5)}>{fields[5].fieldPicked ? "" : fields[5].word}</button></td>
                                <td><button style={this.returnColor(6)} onClick={() => this.onClick(6)}>{fields[6].fieldPicked ? "" : fields[6].word}</button></td>
                                <td><button style={this.returnColor(7)} onClick={() => this.onClick(7)}>{fields[7].fieldPicked ? "" : fields[7].word}</button></td>
                                <td><button style={this.returnColor(8)} onClick={() => this.onClick(8)}>{fields[8].fieldPicked ? "" : fields[8].word}</button></td>
                                <td><button style={this.returnColor(9)} onClick={() => this.onClick(9)}>{fields[9].fieldPicked ? "" : fields[9].word}</button></td>
                            </tr>
                            <tr>
                                <td><button style={this.returnColor(10)} onClick={() => this.onClick(10)}>{fields[10].fieldPicked ? "" : fields[10].word}</button></td>
                                <td><button style={this.returnColor(11)} onClick={() => this.onClick(11)}>{fields[11].fieldPicked ? "" : fields[11].word}</button></td>
                                <td><button style={this.returnColor(12)} onClick={() => this.onClick(12)}>{fields[12].fieldPicked ? "" : fields[12].word}</button></td>
                                <td><button style={this.returnColor(13)} onClick={() => this.onClick(13)}>{fields[13].fieldPicked ? "" : fields[13].word}</button></td>
                                <td><button style={this.returnColor(14)} onClick={() => this.onClick(14)}>{fields[14].fieldPicked ? "" : fields[14].word}</button></td>
                            </tr>
                            <tr>
                                <td><button style={this.returnColor(15)} onClick={() => this.onClick(15)}>{fields[15].fieldPicked ? "" : fields[15].word}</button></td>
                                <td><button style={this.returnColor(16)} onClick={() => this.onClick(16)}>{fields[16].fieldPicked ? "" : fields[16].word}</button></td>
                                <td><button style={this.returnColor(17)} onClick={() => this.onClick(17)}>{fields[17].fieldPicked ? "" : fields[17].word}</button></td>
                                <td><button style={this.returnColor(18)} onClick={() => this.onClick(18)}>{fields[18].fieldPicked ? "" : fields[18].word}</button></td>
                                <td><button style={this.returnColor(19)} onClick={() => this.onClick(19)}>{fields[19].fieldPicked ? "" : fields[19].word}</button></td>
                            </tr>
                            <tr>
                                <td><button style={this.returnColor(20)} onClick={() => this.onClick(20)}>{fields[20].fieldPicked ? "" : fields[20].word}</button></td>
                                <td><button style={this.returnColor(21)} onClick={() => this.onClick(21)}>{fields[21].fieldPicked ? "" : fields[21].word}</button></td>
                                <td><button style={this.returnColor(22)} onClick={() => this.onClick(22)}>{fields[22].fieldPicked ? "" : fields[22].word}</button></td>
                                <td><button style={this.returnColor(23)} onClick={() => this.onClick(23)}>{fields[23].fieldPicked ? "" : fields[23].word}</button></td>
                                <td><button style={this.returnColor(24)} onClick={() => this.onClick(24)}>{fields[24].fieldPicked ? "" : fields[24].word}</button></td>
                            </tr>
                        </tbody>
                    </table>
                </ div>
            </div>)
    }

    renderTeamSelector(blue, red) {
        console.log(blue);
        return (
            <div>
                <div className='chooseTeam'>
                    <h1 id="tabelLabel" >Játék</h1>
                </div>

                <div className='chooseTeam'>
                    <p>Válassz egy csapatot és indulhat a játék.</p>
                </div>
                <div className='chooseTeam'>
                    <button className='join-red' onClick={this.onJoinRed}>Csatlakozás a piros csapathoz</button>
                </div>
                <div className='chooseTeam'>
                    <button className='join-blue' onClick={this.onJoinBlue}>Csatlakozás a kék csapathoz</button>
                </div>
                <div className='chooseTeam'>
                    <button className='join-red' onClick={this.onJoinRedAsMaster}>Csatlakozás a piros csapathoz kémfőnökként</button>
                </div>
                <div className='chooseTeam'>
                    <button className='join-blue' onClick={this.onJoinBlueAsMaster}>Csatlakozás a kék csapathoz kémfőnökként</button>
                </div >


                <div className='row'>
                    <div className='column'>
                        <table className='table table-striped' aria-labelledby="tabelLabel">
                            <div className='chooseTeam'>
                                <thead>
                                    <tr>
                                        <th>Kék csapat</th>
                                    </tr>
                                </thead>
                            </div>
                            <tbody>
                                {blue.map(blue =>
                                    <tr key={blue.name}>
                                        <td>{blue.name}</td>
                                        <td>{blue.spymaster.toString() === "true" ? "Kémfőnök" : "Kém"}</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                    <div className='column'>
                        <table className='table table-striped' aria-labelledby="tabelLabel">
                            <div className='chooseTeam'>
                                <thead>
                                    <tr>
                                        <th>Piros csapat</th>
                                    </tr>
                                </thead>
                            </div>
                            <tbody>
                                {red.map(red =>
                                    <tr key={red.name}>
                                        <td>{red.name}</td>
                                        <td>{red.spymaster.toString() === "true" ? "Kémfőnök" : "Kém"}</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
                <div className='chooseTeam'>
                    <button className='start-game' onClick={this.onStartGame}>Játék indítása</button>
                </div>
            </div >
        );

    }
    renderVictory(winner) {
        if (winner === 0) {
            this.setState({
                fields: [], game: [], user: '', team: -1, redteam: [], blueteam: [], redmaster: false, bluemaster: false, ismaster: false, started: false, clue: '', clueno: 0, cluevalid: false, inputVisible: true,
                redpoints: 0, bluepoints: 0, chatMessages: [], message: ''
            })
            return (
                <div className='endscreen-red'>
                    <h1>Vörös nyert</h1>
                </div>
            )
        }
        else if (winner === 1) {
            this.setState({
                fields: [], game: [], user: '', team: -1, redteam: [], blueteam: [], redmaster: false, bluemaster: false, ismaster: false, started: false, clue: '', clueno: 0, cluevalid: false, inputVisible: true,
                redpoints: 0, bluepoints: 0, chatMessages: [], message: ''
            })
            return (
                <div className='endscreen-blue'>
                    <h1>Kék nyert</h1>
                </div>
            )
        }
    }
    render() {
        console.log(this.state.cluevalid);
        let contents = this.renderTeamSelector(this.state.blueteam, this.state.redteam);
        if (this.state.started)
            contents = this.renderGame(this.state.fields);
        if (this.state.gameended !== -1)
            contents = this.renderVictory(this.state.gameended)

        let chat = this.renderChat(this.state.chatMessages);
        if (this.state.gameended !== -1)
            chat = <div></div>

        return (
            <div>
                {contents}
                {chat}
            </div>
        );
    }

    onGameEnded() {
        // eslint-disable-next-line react/no-direct-mutation-state
        this.state.gameended = -1
        this.render()
    }
    async startgame() {
        try {
            await gameConnection.start();
            console.log("SignalR (game) Connected.");
        } catch (err) {
            console.log("Start hiba:" + err);
            setTimeout(this.start, 50000);
        }
    }
    onJoinRed = async (e) => {
        e.preventDefault();
        try {
            if (this.state.team != 0 && this.state.ismaster === false) {
                await gameConnection.invoke('EnterTeamAsOperative', 0);
                this.setState({ team: 0 })
            }
        } catch (err) {
            console.error(err);
        }
    }
    onJoinBlue = async (e) => {
        e.preventDefault();
        try {
            if (this.state.team != 1 && this.state.ismaster === false) {
                await gameConnection.invoke('EnterTeamAsOperative', 1);
                this.setState({ team: 1 })
            }
        } catch (err) {
            console.error(err);
        }
    }
    onJoinRedAsMaster = async (e) => {
        e.preventDefault();
        try {
            if (this.state.redmaster === false && this.state.ismaster === false) {
                await gameConnection.invoke('EnterTeamAsSpymaster', 0)
                this.setState({ team: 0, ismaster: true })
            }
        } catch (err) {
            console.error(err)
        }
    }
    onJoinBlueAsMaster = async (e) => {
        e.preventDefault();
        try {
            if (this.state.bluemaster === false && this.state.ismaster === false) {
                await gameConnection.invoke('EnterTeamAsSpymaster', 1)
                this.setState({ team: 1, ismaster: true })
            }
        } catch (err) {
            console.error(err)
        }
    }

    onStartGame = async (e) => {
        e.preventDefault();
        try {
            if (this.state.blueteam.length >= 2 && this.state.redteam.length >= 2
                && this.state.bluemaster && this.state.redmaster) {

                await gameConnection.invoke('StartGame')

            }
            else if (!this.state.bluemaster)
                alert('Nincs kék kémfőnök')
            else if (!this.state.redmaster)
                alert('Nincs piros kémfőnök')
            else if (this.state.blueteam.length < 2)
                alert('Nincsenek legalább 2-en a kék csapatban ')
            else if (this.state.redteam.length < 2)
                alert('Nincsenek legalább 2-en a piros csapatban ')
        } catch (err) {
            console.error(err)
        }
    }
    returnColor(number) {
        let pictureSelector = Math.floor(Math.random() * 2)
        switch (this.state.fields[number].color) {
            case 1: {
                if (this.state.fields[number].fieldPicked)
                    if (pictureSelector === 0)
                        return { backgroundImage: "url(/assets/piros_ferfi_ugynok.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                    else
                        return { backgroundImage: "url(/assets/piros_no_ugynok.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                if (this.state.ismaster)
                    return { backgroundImage: "url(/assets/card_piros.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                else {
                    return { backgroundImage: "url(/assets/card_sima.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                }
            }
            case 2: {
                if (this.state.fields[number].fieldPicked)
                    if (pictureSelector === 0)
                        return { backgroundImage: "url(/assets/kek_ferfi_ugynok.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                    else
                        return { backgroundImage: "url(/assets/kek_no_ugynok.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                if (this.state.ismaster)
                    return { backgroundImage: "url(/assets/card_kek.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                else
                    return { backgroundImage: "url(/assets/card_sima.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
            }
            case 3: {
                if (this.state.fields[number].fieldPicked)
                    if (pictureSelector === 0)
                        return { backgroundImage: "url(/assets/semleges_ferfi_ugynok.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                    else
                        return { backgroundImage: "url(/assets/semleges_no_ugynok.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                if (this.state.ismaster)
                    return { backgroundImage: "url(/assets/card_sima.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                else
                    return { backgroundImage: "url(/assets/card_sima.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
            }
            case 4: {
                if (this.state.fields[number].fieldPicked)
                    return { backgroundImage: "url(/assets/fekete_ugynok.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
                if (this.state.ismaster)
                    return { backgroundImage: "url(/assets/card_fekete.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', color: 'white', backgroundSize: 'cover' };
                else
                    return { backgroundImage: "url(/assets/card_sima.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
            }
            default: return { backgroundImage: "url(/assets/card_piros.png)", width: '200px', height: '126px', border: 'none', borderRadius: '5px 5px 5px 5px', borderWidth: '0px', textAlign: 'center', bottom: '0', fontSize: '26px', padding: '23% 20% 0% 20%', backgroundSize: 'cover' };
        }

    }

    onClick = async (index) => {
        if (!this.state.ismaster && this.state.team == this.state.game[0].curentTeam && !this.state.fields[index].fieldPicked) {
            await gameConnection.invoke('FieldClicked', index % 5, Math.floor(index / 5), this.state.team)
        }
    }
    onSubmit = async (e) => {
        e.preventDefault();

        if (this.state.clue !== '' && this.state.clueno !== 0) {
            await gameConnection.invoke('IsClueValidInv', this.state.clue, this.state.clueno)
        }

        else {
            alert('Please insert an user and a message.');
        }


    }
    onClueUpdate = (e) => {
        this.setState({ clue: e.target.value });
    }
    onCluenoUpdate = (e) => {
        this.setState({ clueno: e.target.valueAsNumber });
    }

    sendMessage = async (user, message) => {

        const chatMessage = {
            user: user,
            message: message
        }
        try {
            await gameConnection.invoke('SendMessage', chatMessage.user, chatMessage.message);
        } catch (err) {
            console.error(err);
        }
    }
    onSubmitChat = (e) => {
        e.preventDefault();

        const isMessageProvided = this.state.message && this.state.message !== '';

        if (isMessageProvided) {
            this.sendMessage(this.state.user, this.state.message);
        }
        else {
            alert('Please insert a message.');
        }

    }

    onMessageUpdate = (e) => {
        this.setState({ message: e.target.value });
    }
}



