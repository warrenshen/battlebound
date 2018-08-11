import React from "react";
import colours from '../shared/colours';

class SectionHeading extends React.Component {

  render(){

    return (
      <div style={styles.wrapper}>
        <div style={styles.container}>
          <h1 style={Object.assign({}, styles.sectionHeading, this.props.style)}>
            {this.props.title}
          </h1>
          <h4>
            {this.props.subtitle}
          </h4>
        </div>
      </div>
      );

  }

}

export default SectionHeading;

let styles = {};

styles.wrapper = {
  display: "flex",
  flexDirection: "row",
  justifyContent: "center",
  alignItems: "center",
  padding: "36px 0px",
}

styles.container = {
  display: "flex",
  flexDirection: "column",
  justifyContent: "center",
  alignItems: "center",
}

styles.sectionHeading = {
  fontFamily: 'Apercu-Bold',
  textTransform: 'uppercase',
  fontSize: '4.5vh',
  letterSpacing: '0.06em',
  position: "relative",
  display: "inline-block",
  color: colours.blue,
}

styles.span = {
  position: "absolute",
  top: "-3.5vh",
  left: "2px",
  color: "black",
  fontFamily: "RobotoMono-Regular",
  textTransform: "initial",
  fontSize: "2.5vh"
}
