import React, { Component } from 'react';

import './Home.css';
import * as contentful from 'contentful';
import VideoHeader from './components/VideoHeader/VideoHeader.js';
import TotalNumbers from './components/TotalNumbers/TotalNumbers.js';
import CurrentCharts from './components/CurrentCharts/CurrentCharts.js';
import CharitySlider from './components/CharitySlider/CharitySlider.js';
import TeamGoals from './components/TeamGoals/TeamGoals.js';
import HowItWorks from './components/HowItWorks/HowItWorks.js';
import InstaGallery from './components/InstaGallery/InstaGallery.js';
import OtherInitiatives from './components/OtherInitiatives/OtherInitiatives.js';
import Footer from './components/Footer/Footer.js';

import i18n from 'i18n';
import { withNamespaces } from 'react-i18next';
import * as api_public from "../../api/endpoints/public"


class Home extends Component {
  client: null;
  setContentfulEntries = function(){
    this.client.getEntries({locale:this.state.contentfulLang}).then(entries => {
      this.setState({
        contentfulEntries: entries.items
      });
    })
  }

  constructor(props) {
    super(props);

    this.state = {
      bfmStats: '',
      contentfulEntries: '',
      contentfulLang: localStorage.getItem('contentfulLang') || 'en-US',
      lang: localStorage.getItem('language') || 'en'
    };
  }

  render() {
    const changeLanguage = (lng) => {
      let contentfulLang = (lng==='en' ? 'en-US': lng);
      this.setState({
        lang: lng,
        contentfulLang: contentfulLang
      });
      localStorage.setItem('language', lng);
      localStorage.setItem('contentfulLang', contentfulLang);

      i18n.changeLanguage(lng);
    }

    return (
      <div className="Home">
        <div className="Home__langSwitcher">
          <button className={`Home__langSwitcher-Button ${(this.state.lang === 'en' ? 'active' : '')}`} onClick={() => changeLanguage('en') }>en</button>
          <button className={`Home__langSwitcher-Button ${(this.state.lang === 'pl' ? 'active' : '')}`} onClick={() => changeLanguage('pl')}>pl</button>
        </div>

        <VideoHeader/>
        <TotalNumbers data={this.state.bfmStats}/>
        <CurrentCharts data={this.state.bfmStats}/>
        <CharitySlider data={this.state.contentfulEntries}/>
        <TeamGoals/>
        <HowItWorks/>
        <InstaGallery/>
        <OtherInitiatives/>
        <Footer/>
      </div>
    );
  }

  componentDidMount(){
    //contentful
    this.client = contentful.createClient({
      space: "r9sx20y0suod",
      accessToken: "0cfdeec874152c24de8109da60c0bd09630fd3e4efdeddf9223652a433927fc4",
      host: "preview.contentful.com"
    });
    console.log(this.client)
    this.setContentfulEntries();

    api_public.getTotalNumbers()
      .then(
        (result) => { this.setState({ bfmStats: result}); },
        (error) => { this.setState({ bfmStats: null,}); console.error('Error:', error); }
      );
  }
  componentDidUpdate(prevProps, prevState){
    if(this.state.contentfulLang !== prevState.contentfulLang){
      this.setContentfulEntries();
    }
  }
}

export default withNamespaces()(Home);