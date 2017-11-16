import * as React from 'react';
import { IProduct } from '../models/IProduct';

export class Product extends React.Component<{ product: IProduct },{}> {
  public render() {
    const { product } = this.props;
    return <div className="product__wrapper">
        <div className="product__image__wrapper">
          <img className="product__image" src={product.imagePath || 'http://via.placeholder.com/320x240'} alt={product.id}/>
        </div>
        <div className="product__info">
          <p className="product__name">{product.name}</p>
          <p>Characteristics:</p>
          <ul>
            {
              Object.keys(product.properties).map(key => {
                return <li className="product__property" key={key}>{key}: {product.properties[key]}</li>
              })
            }
          </ul>
          <hr/>
          <p className="product__description">{product.description}</p>
        </div>
      </div>
  }
}