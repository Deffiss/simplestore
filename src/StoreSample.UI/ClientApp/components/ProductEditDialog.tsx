import * as React from 'react';
import ReactSelect from 'react-select';
import { find } from 'lodash';
import { ICategory } from '../models/ICategory';
import { IProductCommand } from '../models/IProductCommand';
import CatalogService from '../services/CatalogService';

interface State {
  category: ICategory,
  categories: ICategory[],
  newProduct: IProductCommand,
}

interface Props {
  onSave: Function
}

export class ProductEditDialog extends React.Component<Props,State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      categories: [],
      category: { id: '', name: '', properties: {}},
      newProduct: {
        name: '',
        description: '',
        categoryId: '',
        properties: {}
      }
    }
  }

  public componentWillMount() {
    CatalogService.getCategories().then(categories => {
      const { newProduct } = this.state;
      const newCategory = categories[0];
      this.setState({ categories, category: newCategory, newProduct: { ...newProduct, properties: {...newCategory.properties}, categoryId: newCategory.id} })
    });
  }

  public optionsChange = (value: { label: string, value: string}) => {
    const { categories, newProduct } = this.state;
    const newCategory = find(categories, (c: ICategory) => c.id == value.value);
    this.setState({ category: newCategory, newProduct: { ...newProduct, properties: {...newCategory.properties}, categoryId: newCategory.id} })
  }

  public handleProductSave = () => {
    const { onSave } = this.props;
    CatalogService.postProduct(this.state.newProduct).then(() => onSave());
  }

  public render() {
    const options = this.state.categories.map(cat => {
      return { value: cat.id, label: cat.name }
    })

    const { category, newProduct } = this.state;
    const { onSave } = this.props;
    return <div className="product__edit__dialog" onClick={(e) => onSave()}>
          <div className="dialog__body product__wrapper" onClick={e => e.stopPropagation()}>
            <div className="dialog__header">
              <h3>New Product</h3>
              <hr/>
            </div>
            <div className="dialog__details">
              <div className="edit__field">
                <input placeholder='Name' onChange={
                  (e) => {
                    const { newProduct } = this.state;
                    this.setState({ newProduct: { ...newProduct, name: e.target.value }})
                  }
                } type='text' value={newProduct.name}/>
              </div>
              <div className="edit__field">
                <input placeholder='Description' onChange={
                  (e) => {
                    const { newProduct } = this.state;
                    this.setState({ newProduct: { ...newProduct, description: e.target.value }})
                  }
                } type='text' value={newProduct.description} />
              </div>
              <div>
                <ReactSelect onChange={this.optionsChange} options={options} value={category.id} />
              </div>
              <div>
                <h4>Properties:</h4>
                {
                  Object.keys(newProduct.properties).map(key => {
                    return <div className='edit__field'>
                    <p>
                      {key}:
                    </p>
                    <input type='text' onChange={(e) => {
                      const { newProduct } = this.state;
                      this.setState({ newProduct: { ...newProduct, properties: {...newProduct.properties, [key]: e.target.value }}});
                    }} value={newProduct.properties[key]}/></div>
                  })
                }
              </div>
            </div>
              <div className="save__button__wrapper">
                <button onClick={this.handleProductSave} className="btn btn-success save">Save</button>
              </div>
          </div>
      </div>
  }
}