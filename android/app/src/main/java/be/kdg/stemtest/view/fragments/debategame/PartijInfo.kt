package be.kdg.stemtest.view.fragments.debategame

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.view.fragments.PartyInfoAdapter
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.model.entity.Statement
import com.squareup.picasso.Picasso

class PartijInfo : Fragment() {

    private lateinit var spinner: Spinner
    private lateinit var logo: ImageView
    private lateinit var orientation: TextView
    private lateinit var leader: TextView
    private lateinit var colour: TextView

    private lateinit var recycler: RecyclerView
    private lateinit var viewAdapter: PartyInfoAdapter
    private lateinit var viewManager: LinearLayoutManager

    private lateinit var partyAns: List<PartyAnswer>
    private lateinit var answers: List<StudentAnswer>
    private lateinit var statements: List<Statement>
    private lateinit var parties: List<Party>
    private lateinit var answerOptions: List<AnswerOption>


    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_partij_info, container, false)
    }



    private fun loadOverview(party:Party){
        Picasso.get().load(party.logo).into(logo)
        orientation.text = party.orientation
        leader.text = party.partyLeader
        colour.text = party.colour
    }


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        statements = arguments?.get("statementArg") as List<Statement>
        parties = arguments?.get("parties") as List<Party>
        partyAns = arguments?.get("partyArg") as List<PartyAnswer>
        answers = arguments?.get("answerArg") as List<StudentAnswer>
        answerOptions = arguments?.get("answerOptionsArg") as List<AnswerOption>
        initialiseViews(view)
        spinner.adapter= ArrayAdapter(requireContext(),
            R.layout.support_simple_spinner_dropdown_item,parties.map { i -> i.name }.toTypedArray())
        loadOverview(parties[0])
        loadPartyIntoRecyclerview(parties[0])
        loadRecyclerView()
        addEventListeners()
    }

    private fun addEventListeners() {
        spinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onNothingSelected(parent: AdapterView<*>?) {
            }

            override fun onItemSelected(
                parent: AdapterView<*>,
                view: View,
                position: Int,
                id: Long
            ) {

                val selected = spinner.selectedItem.toString();
                val party = parties.find{p->p.name==selected}
                if (party != null) {
                    loadOverview(party)
                    loadPartyIntoRecyclerview(party)
                }
            }
        }
    }

    private fun loadRecyclerView() {
        viewAdapter.setStatementData(statements)
        viewAdapter.setAnswerData(answers)
        viewAdapter.setAnswerOptionsData(answerOptions)
        recycler.apply {
            layoutManager=viewManager
            adapter=viewAdapter
        }
    }
    private fun loadPartyIntoRecyclerview(party: Party){
        viewAdapter.setPartyAnsData(partyAns.filter { i -> i.partyName==party.name })
    }

    private fun initialiseViews(view: View){
        spinner = view.findViewById(R.id.partySelectSpinner)
        orientation = view.findViewById(R.id.tvPartyInfoOrientatie)
        leader = view.findViewById(R.id.tvPartyInfoLeider)
        logo = view.findViewById(R.id.PartyInfoLogo)
        colour = view.findViewById(R.id.tvPartyInfoColour)
        recycler = view.findViewById(R.id.rcPartyInfo)

        viewManager = LinearLayoutManager(context)
        viewAdapter = PartyInfoAdapter(context)
    }

}
