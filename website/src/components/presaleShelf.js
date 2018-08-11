import React from 'react';
import Link from 'gatsby-link';
import Radium from 'radium';
import Web3 from 'web3';

import sharedStyles from '../shared/sharedStyles';

import ProjectScreenshot from './projectScreenshot';

import SectionHeading from "./sectionHeading";

import tajiTheFearlessImage from '../assets/cards/taji-the-fearless.png';
import poseidensHandmaidenImage from '../assets/cards/poseidens-handmaiden.png';

import menuImage from '../assets/screens/screen-old-menu.png';
import battleImage from '../assets/screens/screen-battle.png';
import marketplaceImage from '../assets/screens/screen-marketplace.png';

class PresaleShelf extends React.Component {

  constructor(props) {
    super(props);

    this.isMetamaskAvailable = false;
    this.account = null;
    this.network = null;
    this.instancesSupply = null;
  }

  componentDidMount() {
    if (typeof web3 !== 'undefined') {
      var web3Instance = new Web3(web3.currentProvider);
      var provider = web3.currentProvider;
      if (provider.isMetaMask === true) {
        this.setState({ isMetamaskAvailable: true });

        web3Instance.eth.net.getId()
          .then((network) => {
            if (network != this.network) {
              this.setState({ network: network });
            }
          });

        var accountInterval = setInterval(() => {
          if (web3Instance.eth.accounts[0] != this.state.account) {
            this.setState({ account: web3Instance.eth.accounts[0] });
          }
        }, 100);

        const contract = web3.eth
          .contract([{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'pure','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_tokenId','type':'uint256'}],'name':'approve','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'instancesSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_tokenId','type':'uint256'}],'name':'transferFrom','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_templateId','type':'uint256'}],'name':'getTemplate','outputs':[{'name':'generation','type':'uint128'},{'name':'power','type':'uint128'},{'name':'name','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'templatesSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_tokenId','type':'uint256'},{'name':'_startingPrice','type':'uint256'},{'name':'_endingPrice','type':'uint256'},{'name':'_duration','type':'uint256'}],'name':'createSaleAuction','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_templateId','type':'uint256'}],'name':'instanceLimit','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'ownerOf','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[],'name':'renounceOwnership','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'tokensOfOwner','outputs':[{'name':'','type':'uint256[]'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'owner','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_cardId','type':'uint256'}],'name':'getCard','outputs':[{'name':'generation','type':'uint128'},{'name':'power','type':'uint128'},{'name':'name','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'pure','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_tokenId','type':'uint256'}],'name':'transfer','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_address','type':'address'}],'name':'setSaleAuction','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_tokenId','type':'uint256'}],'name':'templateIdOf','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'saleAuction','outputs':[{'name':'','type':'address'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_templateId','type':'uint256'},{'name':'_owner','type':'address'}],'name':'mintCard','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'_mintLimit','type':'uint256'},{'name':'_generation','type':'uint128'},{'name':'_power','type':'uint128'},{'name':'_name','type':'string'}],'name':'createTemplate','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':false,'inputs':[{'name':'newOwner','type':'address'}],'name':'transferOwnership','outputs':[],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'templateId','type':'uint256'}],'name':'TemplateCreated','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'owner','type':'address'},{'indexed':false,'name':'cardId','type':'uint256'},{'indexed':false,'name':'templateId','type':'uint256'}],'name':'InstanceMinted','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'from','type':'address'},{'indexed':true,'name':'to','type':'address'},{'indexed':false,'name':'tokenId','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':false,'name':'owner','type':'address'},{'indexed':false,'name':'approved','type':'address'},{'indexed':false,'name':'tokenId','type':'uint256'}],'name':'Approval','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'previousOwner','type':'address'}],'name':'OwnershipRenounced','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'previousOwner','type':'address'},{'indexed':true,'name':'newOwner','type':'address'}],'name':'OwnershipTransferred','type':'event'}])
          .at("0x7525106192a90D039Bfe79b39dAFF86123C5850b");

        contract.instancesSupply.call((err, instancesSupply) => {
          console.log(instancesSupply.valueOf());
        });
        // token.totalSupply.call(function(err, totalSupply) {
        //   // 2. get the number of decimal places used to represent this token
        //   token.decimals.call(function(err, decimals) {
        //       // 3. get the name of the token
        //       token.name.call(function(err, name) {
        //           // 4. get the balance of the account holder
        //           token.balanceOf.call(account,
        //           function(err, balance) {
        //               // update the UI to reflect the data returned from the blockchain
        //               var percentOwned = balance.div(totalSupply).mul(100);
        //               var _divisor = new web3.BigNumber(10).toPower(decimals);
        //               divisor = _divisor;
        //               totalSupply = totalSupply.div(divisor);
        //               balance = balance.div(divisor);
        //               var results = 'Token Name: ' + name + '';
        //               results += 'Total supply: ' + totalSupply.round(5) + '';
        //               results += 'You own ' + balance.round(5) + ' which is ' + percentOwned.round(5) + '% of the total supply';
        //               $("#loader").hide();

        //               $('#results').html(results);
        //           });
        //       });
        //   });
      } else {

      }
    } else {

    }
    // const web3 = new Web3(new Web3.providers.HttpProvider('https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8'));
    // console.log(web3.eth.coinbase);
    // console.log(web3.eth.blockNumber);

    // console.log(web3.eth.accounts);



    if(typeof window !== "undefined"){

      const ScrollMagic = require('ScrollMagic');
      typeof window !== 'undefined' && require('animation.gsap');
      // require('debug.addIndicators');
      const TimelineMax = require('TimelineMax');

      const controller = new ScrollMagic.Controller();

      const tween = new TimelineMax();

      tween.add([
        TweenMax.to("#featured-image", 1, {x: 100, ease: Linear.easeNone}),
        TweenMax.to("#featured-image", 1, {opacity: 1, ease: Linear.easeNone})
        ]);

      var scene = new ScrollMagic.Scene({triggerHook: 0.5, triggerElement: "#work-section", duration: 200})
              .setTween(tween)
              // .setPin("#header-pin", {pushFollowers: true})
              // .addIndicators()
              .addTo(controller);

      const tween2 = new TimelineMax();

      tween2.add([
        TweenMax.to("#featured-info", 1, {y: 100, ease: Linear.easeNone}),
        TweenMax.to("#featured-info", 1, {opacity: 1, ease: Linear.easeNone})
        ]);

      var scene = new ScrollMagic.Scene({triggerHook: 0.4, triggerElement: "#work-section", duration: 200})
              .setTween(tween2)
              // .setPin("#header-pin", {pushFollowers: true})
              // .addIndicators()
              .addTo(controller);

      const tweenWorkProjects = new TimelineMax();

      tweenWorkProjects.add([
        TweenMax.to("#featured-info", 1, {y: 100, ease: Linear.easeNone}),
        TweenMax.to("#featured-info", 1, {opacity: 1, ease: Linear.easeNone})
        ]);

      var scene = new ScrollMagic.Scene({triggerHook: 0.4, triggerElement: "#work-section", duration: 200})
              .setTween(tweenWorkProjects)
              // .setPin("#header-pin", {pushFollowers: true})
              // .addIndicators()
              .addTo(controller);
    }


  }

  render(){
    const {
      isMetamaskAvailable,
      account,
      network,
    } = this.state;
    return (
      // <section id="work-section" className="diagonal clockwise" style={styles.section}>
      <section id="work-section" className="clockwise" style={styles.section}>
        <SectionHeading title={"What is Battlebound?"} subtitle={"Blockchain-backed Trading Card Game"} />
        <div className="container-fluid" style={styles.container}>
          <div className="row no-gutters">
            <div className="col-sm-6" style={styles.presaleSection}>
              <img src={tajiTheFearlessImage} style={styles.featuredImage} />
              <h3>0.56 ETH</h3>
            </div>
            <div className="col-sm-6" style={styles.presaleSection}>
              <img src={poseidensHandmaidenImage} style={styles.featuredImage} />
              <h3>0.76 ETH</h3>
            </div>
          </div>
        </div>
        <h2>{isMetamaskAvailable ? "Metamask available" : "Metamask NOT available"}</h2>
        <h2>{account}</h2>
        <h3>{network}</h3>

        <div style={styles.projectsContainer}>
          <div className="projects-flex">
            <div id="project1" className="project-screenshot">
              <ProjectScreenshot screenshotURL={menuImage} description="Buffalo 7" />
            </div>
            <div id="project2" className="project-screenshot">
              <ProjectScreenshot screenshotURL={battleImage} description="Menu Design: Federal Caf&eacute;" />
            </div>
            <div id="project3" className="project-screenshot">
              <ProjectScreenshot screenshotURL={marketplaceImage} description="Open Market" />
            </div>
            {/* <div id="project4" className="project-screenshot">
              <ProjectScreenshot screenshotURL={jamie1} description="Jamie's Veg Patch" />
            </div>
            <div id="project5" className="project-screenshot">
              <ProjectScreenshot screenshotURL={festivalofyou1} description="Festival Of You" />
            </div>
            <div id="project6" className="project-screenshot">
              <ProjectScreenshot screenshotURL={mercury1} description="Mercury Logistics" />
            </div> */}
          </div>
        </div>
      </section>
    );
  }
}


var styles = {};

styles.section = {
  background: "white",
  position: "relative"
}

styles.container = {
  marginTop: "3%",
  padding: 0,
  marginBottom: "7%"
}

styles.presaleSection = {
  position: "relative",
  display: "flex",
  flexDirection: "column",
  justifyContent: "center",
  alignItems: "center",
}

styles.featuredImage = {
  width: "180px",
  height: "240px",
  position: "relative",
  zIndex: 1
}

styles.p = {
  fontFamily: "Apercu-Regular",
  fontSize: "20px",
  color: "#b2b1b8",
  lineHeight: "1.7"
}

styles.projectsContainer = {
  width: "100%",
  position: "relative",
  paddingBottom: "8%"
}

// styles.projectsFlex = {
//   display: "flex",
//   flexWrap: "wrap",
//   justifyContent: "center"
// }

// styles.screenshot = {
//   display: "inline-block",
//   width: "100%",
//   "@media screen and (min-width: 768px)": {
//     width: "calc(33.3333%)"
//   }
// }

export default Radium(PresaleShelf);
