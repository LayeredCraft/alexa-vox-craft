# Properties Requiring `alwaysOutputArray: false` Converters

These 107 properties previously had explicit converters with `(false)` parameter, enabling single-item optimization.
After changing the factory default to `true` (always output arrays), these properties need explicit registration with `(false)`.

## Format
```csharp
var propName = info.Properties.FirstOrDefault(p => p.Name == "propertyName");
propName?.CustomConverter = new APLValueCollectionConverter<Type>(alwaysOutputArray: false);
```

---

## src/AlexaVoxCraft.Model.Apl/APLComponent.cs
- actions → APLValueCollectionConverter<APLAction>(false)
- entities → GenericSingleOrListConverter<object>(false)
- handleTick → GenericSingleOrListConverter<TickHandler>(false)
- handleVisibilityChange → GenericSingleOrListConverter<VisibilityChangeHandler>(false)
- padding → GenericSingleOrListConverter<int>(false)

## src/AlexaVoxCraft.Model.Apl/APLDocument.cs
- handleKeyDown → APLValueCollectionConverter<APLKeyboardHandler>(false)
- handleKeyUp → APLValueCollectionConverter<APLKeyboardHandler>(false)

## src/AlexaVoxCraft.Model.Apl/APLPageMoveHandler.cs
- commands → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Binding.cs
- commands → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Commands/CommandDefinition.cs
- commands → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Commands/InsertItem.cs
- items → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Commands/OpenURL.cs
- onFail → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Commands/Parallel.cs
- commands → APLValueCollectionConverter<APLCommand>(false)
- data → GenericSingleOrListConverter<object>(false)

## src/AlexaVoxCraft.Model.Apl/Commands/PlayMedia.cs
- source → APLValueCollectionConverter<AudioSource>(false)

## src/AlexaVoxCraft.Model.Apl/Commands/Select.cs
- commands → APLValueCollectionConverter<APLCommand>(false)
- otherwise → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Commands/Sequential.cs
- catch → APLValueCollectionConverter<APLCommand>(false)
- commands → APLValueCollectionConverter<APLCommand>(false)
- data → GenericSingleOrListConverter<object>(false)
- finally → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/ActionableComponent.cs
- handleKeyDown → APLValueCollectionConverter<APLKeyboardHandler>(false)
- handleKeyUp → APLValueCollectionConverter<APLKeyboardHandler>(false)
- onBlur → APLValueCollectionConverter<APLCommand>(false)
- onFocus → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaButton.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaCard.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaCheckbox.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaDetail.cs
- button1PrimaryAction → APLValueCollectionConverter<APLCommand>(false)
- button2PrimaryAction → APLValueCollectionConverter<APLCommand>(false)
- ingredientListItems → APLValueCollectionConverter<IngredientListItem>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaGridList.cs
- listItems → APLValueCollectionConverter<AlexaImageListItem>(false)
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaHeader.cs
- headerBackButtonCommand → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaIconButton.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaImageCaption.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaImageListBase.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaListItem.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaPaginatedList.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaPhoto.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaRadioButton.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaSliderBase.cs
- handleKeyDownCommand → APLValueCollectionConverter<APLCommand>(false)
- onBlurCommand → APLValueCollectionConverter<APLCommand>(false)
- onDownCommand → APLValueCollectionConverter<APLCommand>(false)
- onFocusCommand → APLValueCollectionConverter<APLCommand>(false)
- onMoveCommand → APLValueCollectionConverter<APLCommand>(false)
- onUpCommand → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaSwipeToAction.cs
- button1Command → APLValueCollectionConverter<APLCommand>(false)
- button2Command → APLValueCollectionConverter<APLCommand>(false)
- onButtonsHidden → APLValueCollectionConverter<APLCommand>(false)
- onButtonsShown → APLValueCollectionConverter<APLCommand>(false)
- onSwipeDone → APLValueCollectionConverter<APLCommand>(false)
- onSwipeMove → APLValueCollectionConverter<APLCommand>(false)
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaSwitch.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaTextList.cs
- headerBackButtonCommand → APLValueCollectionConverter<APLCommand>(false)
- onSwipeDone → APLValueCollectionConverter<APLCommand>(false)
- onSwipeMove → APLValueCollectionConverter<APLCommand>(false)
- optionsButton1Command → APLValueCollectionConverter<APLCommand>(false)
- optionsButton2Command → APLValueCollectionConverter<APLCommand>(false)
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaTextWrapping.cs
- primaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/AlexaTransportControls.cs
- secondaryControlsAVGLeft → APLValueCollectionConverter<AVG>(false)
- secondaryControlsAVGRight → APLValueCollectionConverter<AVG>(false)
- secondaryControlsLeftAction → APLValueCollectionConverter<APLCommand>(false)
- secondaryControlsRightAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/EditText.cs
- onSubmit → APLValueCollectionConverter<APLCommand>(false)
- onTextChange → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/GridSequence.cs
- childHeights → APLValueCollectionConverter<APLDimensionValue>(false)
- childWidths → APLValueCollectionConverter<APLDimensionValue>(false)
- data → GenericSingleOrListConverter<object>(false)
- items → APLValueCollectionConverter<APLComponent>(false)
- onScroll → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/Image.cs
- filters → APLValueCollectionConverter<Filter>(false)
- sources → APLValueCollectionConverter<ImageSource>(false)

## src/AlexaVoxCraft.Model.Apl/Components/IngredientListItem.cs
- ingredientsPrimaryAction → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/Pager.cs
- handlePageMove → APLValueCollectionConverter<APLPageMoveHandler>(false)
- items → APLValueCollectionConverter<APLComponent>(false)
- onChildrenChanged → APLValueCollectionConverter<APLCommand>(false)
- onPageChanged → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/ResponsiveTemplate.cs
- headerBackButtonCommand → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/ScrollView.cs
- item → APLValueCollectionConverter<APLComponent>(false)
- onScroll → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/Sequence.cs
- onScroll → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/TouchComponent.cs
- gestures → APLValueCollectionConverter<IGesture>(false)
- onCancel → APLValueCollectionConverter<APLCommand>(false)
- onDown → APLValueCollectionConverter<APLCommand>(false)
- onMove → APLValueCollectionConverter<APLCommand>(false)
- onPress → APLValueCollectionConverter<APLCommand>(false)
- onUp → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Components/TouchWrapper.cs
- item → APLValueCollectionConverter<APLComponent>(false)

## src/AlexaVoxCraft.Model.Apl/Components/Video.cs
- onPause → APLValueCollectionConverter<APLCommand>(false)
- onPlay → APLValueCollectionConverter<APLCommand>(false)
- onTrackUpdate → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Gestures/DoublePress.cs
- onDoublePress → APLValueCollectionConverter<APLCommand>(false)
- onSinglePress → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Gestures/LongPress.cs
- onLongPressEnd → APLValueCollectionConverter<APLCommand>(false)
- onLongPressStart → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Gestures/SwipeAway.cs
- item → APLValueCollectionConverter<APLComponent>(false)
- onSwipeDone → APLValueCollectionConverter<APLCommand>(false)
- onSwipeMove → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/Gestures/Tap.cs
- onTap → APLValueCollectionConverter<APLCommand>(false)

## src/AlexaVoxCraft.Model.Apl/VectorGraphics/AVG.cs
- data → GenericSingleOrListConverter<object>(false)
- items → APLValueCollectionConverter<IAVGItem>(false)

## src/AlexaVoxCraft.Model.Apl/VectorGraphics/AVGGroup.cs
- items → APLValueCollectionConverter<IAVGItem>(false)

## src/AlexaVoxCraft.Model.Apl/VectorGraphics/AVGItem.cs
- filters → APLValueCollectionConverter<Filter>(false)

## src/AlexaVoxCraft.Model.Apl/VectorGraphics/AVGPath.cs
- strokeDashArray → APLValueCollectionConverter<double>(false)

---

**Total:** 107 property converters to re-add

**Note:** Properties using `GenericSingleOrListConverter` are for raw (non-APLValue) properties like `IList<T>`. Most are `APLValueCollectionConverter`.