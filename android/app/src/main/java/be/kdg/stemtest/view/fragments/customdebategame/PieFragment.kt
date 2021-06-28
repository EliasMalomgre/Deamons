package be.kdg.stemtest.view.fragments.customdebategame

import android.content.Context
import android.graphics.Color
import androidx.lifecycle.ViewModelProviders
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.lifecycle.LiveData
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import be.kdg.stemtest.viewmodel.PieViewModel
import be.kdg.stemtest.R
import be.kdg.stemtest.model.entity.AnswerOption
import be.kdg.stemtest.model.entity.CustomDebateGameResult
import be.kdg.stemtest.model.entity.GameSettings
import com.github.mikephil.charting.animation.Easing
import com.github.mikephil.charting.data.PieData
import com.github.mikephil.charting.data.PieDataSet
import com.github.mikephil.charting.data.PieEntry
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import kotlinx.android.synthetic.main.pie_fragment.view.*
import javax.inject.Inject


class PieFragment : Fragment(),HasAndroidInjector {

    

    private lateinit var viewModel: PieViewModel
    private lateinit var answerOptionData : LiveData<List<AnswerOption>>
    private lateinit var gameSettings : LiveData<GameSettings>
    private lateinit var resultData : LiveData<CustomDebateGameResult>
    private var index :Int =0
    private var errorShown =false
    
    

    @Inject
    lateinit var androidInjector: DispatchingAndroidInjector<Any>

    @Inject
    lateinit var viewModelFactory: ViewModelProvider.Factory

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.pie_fragment, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        viewModel = ViewModelProviders.of(this, viewModelFactory)[PieViewModel::class.java]

        var answerOptions: List<AnswerOption> = listOf()
        var values: List<Float>
        var colours: List<String?> = listOf()

            index = requireArguments().getInt("index",0)


        answerOptionData = viewModel.getAnswerOptions(index)
        gameSettings = viewModel.getGameSettings()
        resultData=viewModel.getResults(index)

        val resultDataObserver = Observer<CustomDebateGameResult>{i ->
            if (i.id==-1){
                if (!errorShown){
                    Toast.makeText(context,"Kon geen resultaten ophalen",Toast.LENGTH_LONG).show()
                    errorShown=true
                }
            }else{
                errorShown=false
                values = i.values.toList()
                setPieChartData(view,answerOptions,values,colours)
            }

        }

        val gameSettingsObserver = Observer<GameSettings>{ i ->
            colours = listOf(i.colour1,i.colour2,i.colourSkip,i.colour3,i.colour4,i.colour5,i.colour6)
            resultData.observe(viewLifecycleOwner,resultDataObserver)
        }

        val answerOptionsObserver = Observer<List<AnswerOption>>{ i ->
            if (i.isEmpty()||i==null){

            }else{
                answerOptions = i
                gameSettings.observe(viewLifecycleOwner,gameSettingsObserver)
            }

        }
        answerOptionData.observe(viewLifecycleOwner,answerOptionsObserver)
    }


    private fun setPieChartData(view: View, answerOptions: List<AnswerOption>, values: List<Float>, colours: List<String?>) {
        val listPie = ArrayList<PieEntry>()
        var listColours = ArrayList<Int>()

        for (colour in colours) {
            if (colour != null) {
                    listColours.add(Color.parseColor(colour))
                }
        }



        var i = 0;
        for (i in answerOptions.indices){
            listPie.add(PieEntry(values[i], answerOptions[i].opinion))
        }

        if (listColours.isEmpty()){
            listColours.add(resources.getColor(R.color.Graph1))
            listColours.add(resources.getColor(R.color.Graph2))
            listColours.add(resources.getColor(R.color.Graph3))
            listColours.add(resources.getColor(R.color.Graph4))
            listColours.add(resources.getColor(R.color.Graph5))
            listColours.add(resources.getColor(R.color.Graph6))
            listColours.add(resources.getColor(R.color.Graph7))
        }

        val pieDataSet = PieDataSet(listPie, "Verdeling van de meningen")
        pieDataSet.colors = listColours

        val pieData = PieData(pieDataSet)
        pieData.setValueTextSize(14f)
        view.pieChart.data = pieData

        view.pieChart.setUsePercentValues(true)
        view.pieChart.isDrawHoleEnabled = false
        view.pieChart.description.isEnabled = false
        view.pieChart.setEntryLabelColor(R.color.colorPrimaryDark)
        view.pieChart.animateY(2000, Easing.EaseInOutQuad)
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
         AndroidSupportInjection.inject(this)
    }

   

    override fun androidInjector(): AndroidInjector<Any> {
       return  androidInjector
    }

}
