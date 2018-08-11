import React from 'react';
import Link from 'gatsby-link';

import CircleIcon from './circleIcon';

import sharedStyles from '../shared/sharedStyles';
import colours from '../shared/colours';

import SectionHeading from "./sectionHeading";

import app from '../assets/services-icons/app-icon.png';
import dev from '../assets/services-icons/dev-icon.png';
import logo from '../assets/services-icons/logo-icon.png';
import presentation from '../assets/services-icons/presentation-icon.png';
import print from '../assets/services-icons/print-icon.png';
import web from '../assets/services-icons/web-icon.png';

const circleColour = "#f5f6f7";

const ServicesShelf = () =>
  // <section className="diagonal anticlockwise" style={styles.section}>
  <section className="anticlockwise" style={styles.section}>
    <SectionHeading
      title={"Features"}
      subtitle={"Powered by a Hybrid Decentralized-Centralized Architecture"}
    />
    <div className="container">
      <div className="row">
        <div style={styles.col} className="col-xs-6 col-sm-4">
          <CircleIcon iconURL={web} description="Provable Rarity" />
        </div>
        <div style={styles.col} className="col-xs-6 col-sm-4">
          <CircleIcon iconURL={print} description="True Ownership" />
        </div>
        <div style={styles.col} className="clearfix visible-xs-block"></div>
        <div style={styles.col} className="col-xs-6 col-sm-4">
          <CircleIcon iconURL={logo} description="Real Tradability" />
        </div>
        <div style={styles.col} className="col-xs-6 col-sm-4">
          <CircleIcon iconURL={logo} description="Actual Gameplay" />
        </div>
        <div style={styles.col} className="clearfix visible-sm-block visible-md-block"></div>
      </div>
    </div>
  </section>

var styles = {};

styles.section = {
  background: colours.backgroundGrey,
  paddingBottom: "10%",
  position: "relative"
}

styles.col = {
  paddingLeft: 0,
  paddingRight: 0
}

export default ServicesShelf
