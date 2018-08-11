import React from 'react';
import Link from 'gatsby-link';
import Radium from 'radium';

import sharedStyles from '../shared/sharedStyles';

import ProjectScreenshot from './projectScreenshot';

import SectionHeading from "./sectionHeading";

class WorkShelf extends React.Component {

  componentDidMount(){

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

    return (
      // <section id="work-section" className="diagonal clockwise" style={styles.section}>
      <section id="work-section" className="clockwise" style={styles.section}>
        <SectionHeading title={"What is Battlebound?"} subtitle={"Blockchain-backed Trading Card Game"} />
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

export default Radium(WorkShelf);
