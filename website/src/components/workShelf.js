import React from 'react';
import Link from 'gatsby-link';
import Radium from 'radium';

import sharedStyles from '../shared/sharedStyles';

import ProjectScreenshot from './projectScreenshot';

import SectionHeading from "./sectionHeading";

import tajiTheFearlessImage from "../assets/cards/taji-the-fearless.png";
import poseidensHandmaidenImage from "../assets/cards/poseidens-handmaiden.png";

import reebok1 from '../assets/work/reebok1.jpg';
import buffalo1 from '../assets/work/buffalo7-1.png';
import federal1 from '../assets/work/federal1.jpg';
import openmarket1 from '../assets/work/openmarket1.jpg';
import jamie1 from '../assets/work/jamie1.jpg';
import festivalofyou1 from '../assets/work/festivalofyou1.jpg';
import mercury1 from '../assets/work/mercury1.jpg';

import DottedCircle from './decorations/dottedCircle';
import Circle from './decorations/circle';
import Triangle from './decorations/triangle';

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

    const { allMarkdownRemark } = this.props.data;
    const { edges } = allMarkdownRemark;

    return (<section id="work-section" className="diagonal clockwise" style={styles.section}>
      <SectionHeading title="Presale" />

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

      <div style={styles.projectsContainer}>
        <div className="projects-flex">
          <div id="project1" className="project-screenshot">
            <ProjectScreenshot screenshotURL={buffalo1} description="Buffalo 7" />
          </div>
          <div id="project2" className="project-screenshot">
            <ProjectScreenshot screenshotURL={federal1} description="Menu Design: Federal Caf&eacute;" />
          </div>
          <div id="project3" className="project-screenshot">
            <ProjectScreenshot screenshotURL={openmarket1} description="Open Market" />
          </div>
          <div id="project4" className="project-screenshot">
            <ProjectScreenshot screenshotURL={jamie1} description="Jamie's Veg Patch" />
          </div>
          <div id="project5" className="project-screenshot">
            <ProjectScreenshot screenshotURL={festivalofyou1} description="Festival Of You" />
          </div>
          <div id="project6" className="project-screenshot">
            <ProjectScreenshot screenshotURL={mercury1} description="Mercury Logistics" />
          </div>
        </div>
      </div>

    </section>);
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
