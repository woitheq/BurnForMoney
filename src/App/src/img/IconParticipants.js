import React from "react";

const SVG = ({
  style = {},
  fill = "#9B9B9B",
  width="24px",
  height="21px",
  viewBox="0 0 24 21",
  className = ""
}) => (
  <svg
    width={width}
    style={style}
    height={width}
    viewBox={viewBox}
    xmlns="http://www.w3.org/2000/svg"
    className={`svg-icon ${className || ""}`}
    xmlnsXlink="http://www.w3.org/1999/xlink"
  >
  <g id="Symbols" stroke="none" strokeWidth="1" fill="none" fillRule="evenodd">
      <g id="Side-menu-/-Beneficiaries" transform="translate(-20.000000, -196.000000)">
          <g id="Group-15" transform="translate(20.000000, 195.000000)">
              <g id="account-group-1">
                  <g id="Outline_Icons_1_" transform="translate(0.000000, 1.833333)" stroke={fill} strokeLinejoin="round">
                      <g id="Outline_Icons">
                          <g id="Group">
                              <path d="M9.497,18.7916667 L18.5,18.7916667 C18.5,18.7916667 18.5,15.719 18.026,14.4090833 C17.613,13.26875 14.63,12.5060833 11.5,11.4253333 L11.5,9.13366667 C11.5,9.13366667 12.5,8.56625 12.5,6.38366667 C13.5,6.38366667 13.5,4.55033333 12.5,4.55033333 C12.5,4.33583333 13.578,3.01216667 13.25,1.80033333 C12.776,0.055 7.099,0.055 6.625,1.80033333 C4.256,1.364 5.5,4.25791667 5.5,4.55033333 L5.5,6.38366667 C5.5,8.56625 7.5,9.13366667 7.5,9.13366667 L7.5,11.4253333 C4.722,12.3933333 1.386,13.2696667 0.974,14.4090833 C0.5,15.719 0.5,18.7916667 0.5,18.7916667 L9.497,18.7916667 Z" id="Shape"></path>
                              <path d="M21,18.7916667 L23.5,18.7916667 C23.5,18.7916667 23.5,15.719 23.026,14.4090833 C22.613,13.26875 19.63,12.04775 16.5,10.967 L16.5,9.592 C16.5,9.592 17.5,9.02458333 17.5,6.842 C18.5,6.842 18.5,5.00866667 17.5,5.00866667 C17.5,4.79416667 18.578,3.30183333 18.25,2.09 C18.028,1.27325 16.125,-0.0165 14.625,0.900166667" id="Shape" strokeLinecap="round"></path>
                          </g>
                      </g>
                  </g>
                  <g id="Invisible_Shape">
                      <rect id="Rectangle-path" x="0" y="0" width="24" height="22"></rect>
                  </g>
              </g>
          </g>
      </g>
  </g>
  </svg>
);

export default SVG;
