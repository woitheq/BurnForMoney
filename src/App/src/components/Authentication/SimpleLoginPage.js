import React, {Component} from 'react';
import { AuthManager } from "./AuthManager"

class SimpleLoginPage extends Component {
    constructor(props){
        super(props);
    }

    azureAdLogin(){
        let mgr = new AuthManager();
        mgr.login();
    }

    render(){
        return (
        <div>
            Elo Ziom :) Trzeba sie zalogowac! <br/>
            Albo jak Cześi mawaiją:  Musíte se přihlásit) <br/>
            <button onClick={this.azureAdLogin}>Sign In</button><br/>
        </div>);
    }
}

export default SimpleLoginPage;