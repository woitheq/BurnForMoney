import React, { Component } from 'react';

import './StravaAuth.css';
import logo from 'img/logo.svg';

import arrow from './img/arrow_down.svg';
import bigButtonTop from './img/bigButton_top.svg';
import bigButtonBottom from './img/bigButton_bottom.svg';

class StravaAuth extends Component {
  constructor(props) {
      super(props);
      this.state = { stravaLink: undefined } ;
    }

    componentWillMount() {
      let stravaLink;
      switch(process.env.NODE_ENV){
        case 'development':
          stravaLink = "https://www.strava.com/oauth/authorize?client_id=28597&response_type=code&redirect_uri=http://localhost:7071/api/strava/authorize/&approval_prompt=force";
          break;
        case 'test':
          stravaLink = "https://www.strava.com/oauth/authorize?client_id=28591&response_type=code&redirect_uri=https://burnformoneyfunc-test.azurewebsites.net/api/strava/authorize/&approval_prompt=force";
          break;
        case 'production':
          stravaLink = "https://www.strava.com/oauth/authorize?client_id=26733&response_type=code&redirect_uri=https://burnformoneyfunc-prod.azurewebsites.net/api/strava/authorize/&approval_prompt=force";
          break;
      }
      this.setState({ stravaLink });
    }

  render() {
    return (
      <div className="StravaAuth">
        <img src={logo} alt="Logo" className="logo"/>
        <p>Hi Strava user,<br/>
Authorize Burn For Money to connect to Strava,<br/>
so your points will be automatically counted in our stats.</p>
        <div className="StravaAuth__authorize">
          <p>Authorize</p>
          <img src={arrow} className="StravaAuth__authorize-indicator" alt="authorize below"/>
        </div>
        <a href={this.state.stravaLink} className="StravaAuth__bigButton">
          <img src={bigButtonTop} className="StravaAuth__bigButton-top" alt="big button top"/>
          <img src={bigButtonBottom} className="StravaAuth__bigButton-bottom" alt="big button base"/>
        </a>
      </div>
    );
  }
}

export default StravaAuth;
